using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Cadastro de itens de estoque. O único dos três (Categoria/Grupo/Item) que
/// desativa: só ele tem Active e DeletedAt na entidade.
///
/// Regra de preço: "mestre enxerga quantidade, admin enxerga dinheiro". Quem
/// não tem view_finance recebe StockItemDto (sem ReferencePrice) e nunca tem
/// o preço aplicado em Create/Update, mesmo que o mande no corpo — o back é
/// quem decide, a tela é só a primeira barreira.
/// </summary>
[ApiController]
[Route("api/[controller]")] // vira /api/stockitems
[Authorize(Policy = "view_stock")]
public sealed class StockItemsController(AppDbContext db) : ControllerBase
{
    private const int MaxPageSize = 100;

    private bool CanSeePrice => User.HasClaim("permission", "view_finance");

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null,
        [FromQuery] long? stockGroupId = null)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var query = db.StockItem
            .AsNoTracking()
            .Include(i => i.StockGroup)
            .AsQueryable();

        if (stockGroupId is not null)
        {
            query = query.Where(i => i.StockGroupId == stockGroupId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(i => i.Name.Contains(term) || (i.Code != null && i.Code.Contains(term)));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(i => i.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        // O tipo do item muda conforme quem pergunta — por isso o corpo é
        // montado aqui em vez de deixar o ActionResult<T> genérico decidir.
        object dtoItems = CanSeePrice
            ? items.Select(StockItemWithPriceDto.FromEntity).ToArray()
            : items.Select(StockItemDto.FromEntity).ToArray();

        return Ok(new
        {
            items = dtoItems,
            page,
            size,
            total,
            totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size)),
        });
    }

    [HttpPost]
    [Authorize(Policy = "manage_stock")]
    public async Task<IActionResult> Create(CreateStockItemRequest request)
    {
        var group = await ValidateAsync(request.Name, request.Unit, request.MinQuantity, request.StockGroupId, request.ReferencePrice);
        if (group is null) return ValidationProblem(ModelState);

        var item = new StockItem
        {
            StockGroupId = group.Id,
            Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim(),
            Name = request.Name.Trim(),
            Unit = request.Unit.Trim(),
            MinQuantity = request.MinQuantity,
            Ca = string.IsNullOrWhiteSpace(request.Ca) ? null : request.Ca.Trim(),
            CaExpiryDate = request.CaExpiryDate,
            Active = true,
            // Quem não tem view_finance nunca define o preço: o item nasce
            // com 0, e fica para quem tem a permissão editar depois.
            ReferencePrice = CanSeePrice ? (request.ReferencePrice ?? 0) : 0,
        };

        db.StockItem.Add(item);
        await db.SaveChangesAsync();
        item.StockGroup = group; // evita um SELECT extra só para montar o DTO

        return StatusCode(StatusCodes.Status201Created, ToDto(item));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "manage_stock")]
    public async Task<IActionResult> Update(long id, UpdateStockItemRequest request)
    {
        var item = await db.StockItem
            .Include(i => i.StockGroup)
            .SingleOrDefaultAsync(i => i.Id == id);

        if (item is null) return NotFound();

        var group = await ValidateAsync(request.Name, request.Unit, request.MinQuantity, request.StockGroupId, request.ReferencePrice);
        if (group is null) return ValidationProblem(ModelState);

        item.StockGroupId = group.Id;
        item.StockGroup = group;
        item.Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim();
        item.Name = request.Name.Trim();
        item.Unit = request.Unit.Trim();
        item.MinQuantity = request.MinQuantity;
        item.Ca = string.IsNullOrWhiteSpace(request.Ca) ? null : request.Ca.Trim();
        item.CaExpiryDate = request.CaExpiryDate;

        // Quem não tem view_finance manda o request sem esse campo (ou com
        // qualquer coisa nele) — o valor é sempre ignorado, o preço atual
        // fica como estava.
        if (CanSeePrice && request.ReferencePrice is not null)
        {
            item.ReferencePrice = request.ReferencePrice.Value;
        }

        await db.SaveChangesAsync();

        return Ok(ToDto(item));
    }

    [HttpPatch("{id:long}/activate")]
    [Authorize(Policy = "manage_stock")]
    public Task<IActionResult> Activate(long id) => SetActiveAsync(id, true);

    [HttpPatch("{id:long}/deactivate")]
    [Authorize(Policy = "manage_stock")]
    public Task<IActionResult> Deactivate(long id) => SetActiveAsync(id, false);

    private async Task<IActionResult> SetActiveAsync(long id, bool active)
    {
        var item = await db.StockItem.SingleOrDefaultAsync(i => i.Id == id);

        if (item is null) return NotFound();

        item.Active = active;
        await db.SaveChangesAsync();

        return NoContent();
    }

    private object ToDto(StockItem item) =>
        CanSeePrice ? StockItemWithPriceDto.FromEntity(item) : StockItemDto.FromEntity(item);

    // Valida campos e devolve o grupo (ou null, já com os erros no ModelState).
    private async Task<StockGroup?> ValidateAsync(
        string name, string unit, decimal minQuantity, long stockGroupId, decimal? referencePrice)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(unit))
        {
            ModelState.AddModelError("unit", "Unit is required.");
        }

        if (minQuantity < 0)
        {
            ModelState.AddModelError("minQuantity", "Minimum quantity cannot be negative.");
        }

        // O banco não tem CHECK constraint (é regra do projeto: validação
        // vira código em vez de SQL escrito à mão), então isso é obrigação
        // do controller.
        if (referencePrice is < 0)
        {
            ModelState.AddModelError("referencePrice", "Price cannot be negative.");
        }

        var group = await db.StockGroup.Include(g => g.StockCategory).SingleOrDefaultAsync(g => g.Id == stockGroupId);

        if (group is null)
        {
            ModelState.AddModelError("stockGroupId", "Pick a group.");
        }

        return ModelState.IsValid ? group : null;
    }
}