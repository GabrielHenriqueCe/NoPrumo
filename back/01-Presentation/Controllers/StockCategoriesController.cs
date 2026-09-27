using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Services; // onde o UsersController encontra o PagedResult
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Cadastro de categorias de estoque. Não há desativar: a entidade não tem
/// coluna Active/DeletedAt.
/// </summary>
[ApiController]
[Route("api/[controller]")] // vira /api/stockcategories
[Authorize(Policy = "view_stock")] // o mínimo para abrir a tela
public sealed class StockCategoriesController(AppDbContext db) : ControllerBase
{
    private const int MaxPageSize = 100;

    [HttpGet]
    public async Task<ActionResult<PagedResult<StockCategoryDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        // Os dois vêm da query string, ou seja, de fora: trave os limites.
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        // AsNoTracking: é só leitura, o EF não precisa guardar estado.
        var query = db.StockCategory.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(c => c.Name.Contains(term));
        }

        // Conta antes de paginar: o total é do filtro inteiro, não da página.
        var total = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<StockCategoryDto>(
            items.Select(StockCategoryDto.FromEntity).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpPost]
    [Authorize(Policy = "manage_stock")] // escrever exige o outro verbo
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

        db.StockCategory.Add(category);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, StockCategoryDto.FromEntity(category));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "manage_stock")]
    public async Task<ActionResult<StockCategoryDto>> Update(long id, UpdateStockCategoryRequest request)
    {
        var category = await db.StockCategory.SingleOrDefaultAsync(c => c.Id == id);

        if (category is null) return NotFound();

        var name = request.Name.Trim();

        // Passa o próprio id para o nome dela mesma não contar como duplicado.
        if (!await IsNameValidAsync(name, id)) return ValidationProblem(ModelState);

        category.Name = name;
        category.TracksProjectBalance = request.TracksProjectBalance;
        category.RequiresReturn = request.RequiresReturn;

        await db.SaveChangesAsync();

        return Ok(StockCategoryDto.FromEntity(category));
    }

    // Valida o nome. O erro é registrado com o nome do campo ("name"), e é isso
    // que faz a mensagem aparecer embaixo do campo certo na tela.
    private async Task<bool> IsNameValidAsync(string name, long ignoreId = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Name is required.");
            return false;
        }

        if (await db.StockCategory.AnyAsync(c => c.Name == name && c.Id != ignoreId))
        {
            ModelState.AddModelError("name", "This category name is already in use.");
            return false;
        }

        return true;
    }
}