using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Gestão de fornecedores.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_purchases")]
public sealed class SuppliersController(AppDbContext db) : ControllerBase
{
    private const int MaxPageSize = 100;

    [HttpGet]
    public async Task<ActionResult<PagedResult<SupplierDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var query = db.Supplier
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(s =>
                s.Name.Contains(term) ||
                (s.DocumentMasked != null && s.DocumentMasked.Contains(term)) ||
                (s.Email != null && s.Email.Contains(term)) ||
                (s.ContactName != null && s.ContactName.Contains(term)));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<SupplierDto>(
            items.Select(SupplierDto.FromEntity).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<SupplierDto>> GetById(long id)
    {
        var supplier = await db.Supplier
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier is null) return NotFound();

        return Ok(SupplierDto.FromEntity(supplier));
    }

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create(CreateSupplierRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var supplier = new Supplier
        {
            Name = request.Name.Trim(),
            DocumentMasked = string.IsNullOrWhiteSpace(request.DocumentMasked) ? null : request.DocumentMasked.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            ContactName = string.IsNullOrWhiteSpace(request.ContactName) ? null : request.ContactName.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        db.Supplier.Add(supplier);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, SupplierDto.FromEntity(supplier));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<SupplierDto>> Update(long id, UpdateSupplierRequest request)
    {
        var supplier = await db.Supplier.SingleOrDefaultAsync(s => s.Id == id);
        if (supplier is null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        supplier.Name = request.Name.Trim();
        supplier.DocumentMasked = string.IsNullOrWhiteSpace(request.DocumentMasked) ? null : request.DocumentMasked.Trim();
        supplier.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        supplier.ContactName = string.IsNullOrWhiteSpace(request.ContactName) ? null : request.ContactName.Trim();
        supplier.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        supplier.City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();
        supplier.State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim();
        supplier.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        supplier.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(SupplierDto.FromEntity(supplier));
    }

    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        var supplier = await db.Supplier.SingleOrDefaultAsync(s => s.Id == id);
        if (supplier is null) return NotFound();

        supplier.Active = true;
        supplier.DeletedAt = null;
        supplier.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var supplier = await db.Supplier.SingleOrDefaultAsync(s => s.Id == id);
        if (supplier is null) return NotFound();

        supplier.Active = false;
        supplier.DeletedAt = DateTime.UtcNow;
        supplier.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return NoContent();
    }
}
