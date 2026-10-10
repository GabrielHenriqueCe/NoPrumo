using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Entrada e saída de estoque. Não há edição nem exclusão: um lançamento
/// errado se corrige com outro lançamento de sinal oposto, nunca alterando o
/// que já aconteceu.
///
/// O saldo nunca é lido de uma coluna — é sempre a soma das movimentações.
/// Para categorias com saldo por obra (consumable material), a soma é
/// filtrada por obra; para as demais (tool), é do item inteiro.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "view_stock")]
public sealed class StockMovementsController(AppDbContext appDbContext) : ControllerBase
{
    private const int MaxPageSize = 100;

    [HttpGet]
    public async Task<ActionResult<PagedResult<StockMovementDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] long? stockItemId = null,
        [FromQuery] long? projectId = null)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var query = appDbContext.StockMovement
            .AsNoTracking()
            .Include(movement => movement.StockItem)
            .Include(movement => movement.Project)
            .Include(movement => movement.Employee)
            .AsQueryable();

        if (stockItemId is not null)
        {
            query = query.Where(movement => movement.StockItemId == stockItemId);
        }

        if (projectId is not null)
        {
            query = query.Where(movement => movement.ProjectId == projectId);
        }

        var total = await query.CountAsync();

        var movements = await query
            .OrderByDescending(movement => movement.Date)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<StockMovementDto>(
            movements.Select(ToDto).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpGet("balance")]
    public async Task<ActionResult<StockBalanceDto>> GetBalance(
        [FromQuery] long stockItemId,
        [FromQuery] long? projectId = null)
    {
        var item = await appDbContext.StockItem
            .Include(item => item.StockGroup)
                .ThenInclude(group => group.StockCategory)
            .SingleOrDefaultAsync(item => item.Id == stockItemId);

        if (item is null) return NotFound();

        var scopeByProject = item.StockGroup.StockCategory.TracksProjectBalance;
        var effectiveProjectId = scopeByProject ? projectId : null;

        var balance = await GetBalanceAsync(stockItemId, effectiveProjectId);

        string? projectName = effectiveProjectId is null
            ? null
            : await appDbContext.Project
                .Where(project => project.Id == effectiveProjectId)
                .Select(project => project.Name)
                .SingleOrDefaultAsync();

        return Ok(new StockBalanceDto(item.Id, item.Name, item.Unit, effectiveProjectId, projectName, balance));
    }

    [HttpGet("below-minimum")]
    public async Task<ActionResult<IReadOnlyList<StockBelowMinimumDto>>> ListBelowMinimum()
    {
        // Duas consultas só: uma soma tudo agrupado por item, a outra traz os
        // itens ativos. Junta em memória — evita uma query por item.
        var balancesByItem = await appDbContext.StockMovement
            .GroupBy(movement => movement.StockItemId)
            .Select(group => new
            {
                StockItemId = group.Key,
                Balance = group.Sum(movement => movement.Type == StockMovementType.Entry ? movement.Quantity : -movement.Quantity),
            })
            .ToDictionaryAsync(entry => entry.StockItemId, entry => entry.Balance);

        var activeItems = await appDbContext.StockItem
            .AsNoTracking()
            .Where(item => item.Active == true)
            .ToListAsync();

        return Ok(activeItems
            .Select(item => new
            {
                item,
                balance = balancesByItem.GetValueOrDefault(item.Id, 0m),
            })
            .Where(entry => entry.balance < entry.item.MinQuantity)
            .Select(entry => new StockBelowMinimumDto(entry.item.Id, entry.item.Name, entry.item.Unit, entry.item.MinQuantity, entry.balance))
            .ToArray());
    }

    [HttpPost]
    [Authorize(Policy = "manage_stock")]
    public async Task<ActionResult<StockMovementDto>> Create(CreateStockMovementRequest request)
    {
        var item = await appDbContext.StockItem
            .Include(item => item.StockGroup)
                .ThenInclude(group => group.StockCategory)
            .SingleOrDefaultAsync(item => item.Id == request.StockItemId);

        if (item is null)
        {
            ModelState.AddModelError("stockItemId", "Pick an item.");
            return ValidationProblem(ModelState);
        }

        if (item.Active != true)
        {
            ModelState.AddModelError("stockItemId", "This item is inactive.");
        }

        if (request.Quantity <= 0)
        {
            ModelState.AddModelError("quantity", "Quantity must be greater than zero.");
        }

        var category = item.StockGroup.StockCategory;

        // Nem por obra nem com devolução: é PPE, que tem fluxo próprio
        // (PpeIssue) e não passa por aqui.
        if (!category.TracksProjectBalance && !category.RequiresReturn)
        {
            ModelState.AddModelError("stockItemId", "This category does not use stock movements.");
            return ValidationProblem(ModelState);
        }

        Project? project = null;
        Employee? employee = null;

        if (category.TracksProjectBalance)
        {
            project = request.ProjectId is null
                ? null
                : await appDbContext.Project.SingleOrDefaultAsync(project => project.Id == request.ProjectId);

            if (project is null)
            {
                ModelState.AddModelError("projectId", "Pick a project.");
            }
        }

        if (category.RequiresReturn)
        {
            employee = request.EmployeeId is null
                ? null
                : await appDbContext.Employees.SingleOrDefaultAsync(employee => employee.Id == request.EmployeeId);

            // Só é obrigatório na saída: é aí que a ferramenta fica com alguém. Numa
            // entrada (compra inicial, ou devolução sem registrar quem devolveu),
            // o funcionário é opcional.
            if (request.Type == StockMovementType.Exit && employee is null)
            {
                ModelState.AddModelError("employeeId", "Pick an employee.");
            }
            else if (request.EmployeeId is not null && employee is null)
            {
                ModelState.AddModelError("employeeId", "Employee not found.");
            }
        }

        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        if (request.Type == StockMovementType.Exit)
        {
            var balance = await GetBalanceAsync(item.Id, category.TracksProjectBalance ? project!.Id : null);

            if (request.Quantity > balance)
            {
                ModelState.AddModelError("quantity", "Not enough balance for this exit.");
                return ValidationProblem(ModelState);
            }
        }

        var movement = new StockMovement
        {
            StockItemId = item.Id,
            StockItem = item,
            Type = request.Type,
            Quantity = request.Quantity,
            Unit = item.Unit,
            Project = project,
            Employee = employee,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            Date = DateTime.Now,
        };

        appDbContext.StockMovement.Add(movement);
        await appDbContext.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, ToDto(movement));
    }

    private async Task<decimal> GetBalanceAsync(long stockItemId, long? projectId)
    {
        var query = appDbContext.StockMovement.Where(movement => movement.StockItemId == stockItemId);

        if (projectId is not null)
        {
            query = query.Where(movement => movement.ProjectId == projectId);
        }

        return await query.SumAsync(movement => movement.Type == StockMovementType.Entry ? movement.Quantity : -movement.Quantity);
    }

    private static StockMovementDto ToDto(StockMovement movement) => new(
        movement.Id,
        movement.StockItemId,
        movement.StockItem.Name,
        movement.Type,
        movement.Quantity,
        movement.Unit,
        movement.ProjectId,
        movement.Project?.Name,
        movement.EmployeeId,
        movement.Employee?.Name,
        movement.Notes,
        movement.Date);
}