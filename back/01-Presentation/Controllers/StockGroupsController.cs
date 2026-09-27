using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Cadastro de grupos de estoque. Não há desativar: a entidade não tem
/// Active/DeletedAt, e apagar quebraria a FK dos itens.
/// </summary>
[ApiController]
[Route("api/[controller]")] // vira /api/stockgroups
[Authorize(Policy = "view_stock")]
public sealed class StockGroupsController(AppDbContext db) : ControllerBase
{
    private const int MaxPageSize = 100;

    [HttpGet]
    public async Task<ActionResult<PagedResult<StockGroupDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null,
        [FromQuery] long? stockCategoryId = null)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        // Include: o DTO precisa do nome da categoria.
        var query = db.StockGroup
            .AsNoTracking()
            .Include(g => g.StockCategory)
            .AsQueryable();

        if (stockCategoryId is not null)
        {
            query = query.Where(g => g.StockCategoryId == stockCategoryId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(g => g.Name.Contains(term));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(g => g.StockCategory.Name)
            .ThenBy(g => g.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<StockGroupDto>(
            items.Select(StockGroupDto.FromEntity).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpPost]
    [Authorize(Policy = "manage_stock")]
    public async Task<ActionResult<StockGroupDto>> Create(CreateStockGroupRequest request)
    {
        var name = request.Name.Trim();

        // Devolve a categoria (ou null, já com o erro registrado no ModelState).
        var category = await ValidateAsync(name, request.StockCategoryId);
        if (category is null) return ValidationProblem(ModelState);

        var group = new StockGroup
        {
            Name = name,
            StockCategory = category, // o EF preenche StockCategoryId sozinho
        };

        db.StockGroup.Add(group);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, StockGroupDto.FromEntity(group));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "manage_stock")]
    public async Task<ActionResult<StockGroupDto>> Update(long id, UpdateStockGroupRequest request)
    {
        var group = await db.StockGroup
            .Include(g => g.StockCategory)
            .SingleOrDefaultAsync(g => g.Id == id);

        if (group is null) return NotFound();

        var name = request.Name.Trim();

        var category = await ValidateAsync(name, request.StockCategoryId, id);
        if (category is null) return ValidationProblem(ModelState);

        // Mover um grupo com itens mudaria o fluxo deles (saldo por obra,
        // devolução...) e bagunçaria o histórico de movimentação.
        if (category.Id != group.StockCategoryId
            && await db.StockItem.AnyAsync(i => i.StockGroupId == id))
        {
            ModelState.AddModelError("stockCategoryId", "This group already has items, so it cannot change category.");
            return ValidationProblem(ModelState);
        }

        group.Name = name;
        group.StockCategory = category;

        await db.SaveChangesAsync();

        return Ok(StockGroupDto.FromEntity(group));
    }

    // Valida nome e categoria. Cada erro leva o nome do campo, e é isso que
    // faz a mensagem aparecer embaixo do campo certo na tela.
    private async Task<StockCategory?> ValidateAsync(string name, long stockCategoryId, long ignoreId = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }

        var category = await db.StockCategory.SingleOrDefaultAsync(c => c.Id == stockCategoryId);

        if (category is null)
        {
            ModelState.AddModelError("stockCategoryId", "Pick a category.");
        }

        // Nome repetido só incomoda dentro da mesma categoria.
        if (category is not null
            && !string.IsNullOrWhiteSpace(name)
            && await db.StockGroup.AnyAsync(g =>
                g.Name == name && g.StockCategoryId == stockCategoryId && g.Id != ignoreId))
        {
            ModelState.AddModelError("name", "This category already has a group with this name.");
        }

        return ModelState.IsValid ? category : null;
    }
}