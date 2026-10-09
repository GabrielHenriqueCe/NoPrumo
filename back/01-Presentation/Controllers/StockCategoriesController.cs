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
/// Cadastro de categorias de estoque. Não há desativar: a entidade não tem
/// coluna Active/DeletedAt.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "view_stock")]
public sealed class StockCategoriesController(AppDbContext appDbContext) : ControllerBase
{
    private const int MaxPageSize = 100;

    [HttpGet]
    public async Task<ActionResult<PagedResult<StockCategoryDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var query = appDbContext.StockCategory.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(category => category.Name.Contains(term));
        }

        var total = await query.CountAsync();

        var categories = await query
            .OrderBy(category => category.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<StockCategoryDto>(
            categories.Select(ToDto).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpPost]
    [Authorize(Policy = "manage_stock")]
    public async Task<ActionResult<StockCategoryDto>> Create(CreateStockCategoryRequest request)
    {
        var name = request.Name.Trim();

        if (!await IsNameValidAsync(name)) return ValidationProblem(ModelState);

        var category = new StockCategory
        {
            Name = name,
            TracksProjectBalance = request.TracksProjectBalance,
            RequiresReturn = request.RequiresReturn,
        };

        appDbContext.StockCategory.Add(category);
        await appDbContext.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, ToDto(category));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "manage_stock")]
    public async Task<ActionResult<StockCategoryDto>> Update(long id, UpdateStockCategoryRequest request)
    {
        var category = await appDbContext.StockCategory.SingleOrDefaultAsync(category => category.Id == id);

        if (category is null) return NotFound();

        var name = request.Name.Trim();

        if (!await IsNameValidAsync(name, id)) return ValidationProblem(ModelState);

        category.Name = name;
        category.TracksProjectBalance = request.TracksProjectBalance;
        category.RequiresReturn = request.RequiresReturn;

        await appDbContext.SaveChangesAsync();

        return Ok(ToDto(category));
    }

    private static StockCategoryDto ToDto(StockCategory category) => new(
        category.Id,
        category.Name,
        category.TracksProjectBalance,
        category.RequiresReturn);

    private async Task<bool> IsNameValidAsync(string name, long ignoreId = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Name is required.");
            return false;
        }

        if (await appDbContext.StockCategory.AnyAsync(category => category.Name == name && category.Id != ignoreId))
        {
            ModelState.AddModelError("name", "This category name is already in use.");
            return false;
        }

        return true;
    }
}