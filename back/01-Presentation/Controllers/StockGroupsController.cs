using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Cadastro de grupos de estoque. Não há desativar: a entidade não tem
/// Active/DeletedAt, e apagar quebraria a FK dos itens.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "view_stock")]
public sealed class StockGroupsController(AppDbContext appDbContext) : ControllerBase
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

        var query = appDbContext.StockGroup
            .AsNoTracking()
            .Include(group => group.StockCategory)
            .AsQueryable();

        if (stockCategoryId is not null)
        {
            query = query.Where(group => group.StockCategoryId == stockCategoryId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(group => group.Name.Contains(term));
        }

        var total = await query.CountAsync();

        var groups = await query
            .OrderBy(group => group.StockCategory.Name)
            .ThenBy(group => group.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<StockGroupDto>(
            groups.Select(ToDto).ToArray(),
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

        var category = await ValidateAsync(name, request.StockCategoryId);
        if (category is null) return ValidationProblem(ModelState);

        var group = new StockGroup
        {
            Name = name,
            StockCategory = category,
        };

        appDbContext.StockGroup.Add(group);
        await appDbContext.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, ToDto(group));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "manage_stock")]
    public async Task<ActionResult<StockGroupDto>> Update(long id, UpdateStockGroupRequest request)
    {
        var group = await appDbContext.StockGroup
            .Include(group => group.StockCategory)
            .SingleOrDefaultAsync(group => group.Id == id);

        if (group is null) return NotFound();

        var name = request.Name.Trim();

        var category = await ValidateAsync(name, request.StockCategoryId, id);
        if (category is null) return ValidationProblem(ModelState);

        if (category.Id != group.StockCategoryId
            && await appDbContext.StockItem.AnyAsync(item => item.StockGroupId == id))
        {
            ModelState.AddModelError("stockCategoryId", "This group already has items, so it cannot change category.");
            return ValidationProblem(ModelState);
        }

        group.Name = name;
        group.StockCategory = category;

        await appDbContext.SaveChangesAsync();

        return Ok(ToDto(group));
    }

    private static StockGroupDto ToDto(StockGroup group) => new(
        group.Id,
        group.Name,
        group.StockCategoryId,
        group.StockCategory.Name);

    private async Task<StockCategory?> ValidateAsync(string name, long stockCategoryId, long ignoreId = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }

        var category = await appDbContext.StockCategory.SingleOrDefaultAsync(category => category.Id == stockCategoryId);

        if (category is null)
        {
            ModelState.AddModelError("stockCategoryId", "Pick a category.");
        }

        if (category is not null
            && !string.IsNullOrWhiteSpace(name)
            && await appDbContext.StockGroup.AnyAsync(group =>
                group.Name == name && group.StockCategoryId == stockCategoryId && group.Id != ignoreId))
        {
            ModelState.AddModelError("name", "This category already has a group with this name.");
        }

        return ModelState.IsValid ? category : null;
    }
}