using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Gestão de fornecedores.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_purchases")]
public sealed class SuppliersController(AppDbContext db, DocumentSettings docSettings) : ControllerBase
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
        ValidateSupplierInput(request.Name, request.Email, request.ContactName, request.Phone, request.City, request.State);

        ProcessedDocument? processedDoc = null;
        if (!string.IsNullOrWhiteSpace(request.Document))
        {
            if (!DocumentProcessor.IsValid(request.Document))
            {
                ModelState.AddModelError("document", "Invalid CPF or CNPJ format.");
            }
            else
            {
                processedDoc = DocumentProcessor.Process(request.Document, docSettings.EncryptionKey, docSettings.HmacKey);
                if (processedDoc.Hash != null && await db.Supplier.AnyAsync(s => s.DocumentHash == processedDoc.Hash && s.DeletedAt == null))
                {
                    ModelState.AddModelError("document", "This document is already registered.");
                }
            }
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var supplier = new Supplier
        {
            Name = request.Name.Trim(),
            DocumentEncrypted = processedDoc?.Encrypted,
            DocumentHash = processedDoc?.Hash,
            DocumentMasked = processedDoc?.Masked,
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            ContactName = string.IsNullOrWhiteSpace(request.ContactName) ? null : request.ContactName.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim().ToUpperInvariant(),
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

        ValidateSupplierInput(request.Name, request.Email, request.ContactName, request.Phone, request.City, request.State);

        ProcessedDocument? processedDoc = null;
        bool hasNewDocument = !string.IsNullOrWhiteSpace(request.Document);

        if (hasNewDocument)
        {
            if (!DocumentProcessor.IsValid(request.Document))
            {
                ModelState.AddModelError("document", "Invalid CPF or CNPJ format.");
            }
            else
            {
                processedDoc = DocumentProcessor.Process(request.Document, docSettings.EncryptionKey, docSettings.HmacKey);
                if (processedDoc.Hash != null && await db.Supplier.AnyAsync(s => s.DocumentHash == processedDoc.Hash && s.Id != id && s.DeletedAt == null))
                {
                    ModelState.AddModelError("document", "This document is already registered.");
                }
            }
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        supplier.Name = request.Name.Trim();

        if (hasNewDocument && processedDoc != null)
        {
            supplier.DocumentEncrypted = processedDoc.Encrypted;
            supplier.DocumentHash = processedDoc.Hash;
            supplier.DocumentMasked = processedDoc.Masked;
        }

        supplier.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        supplier.ContactName = string.IsNullOrWhiteSpace(request.ContactName) ? null : request.ContactName.Trim();
        supplier.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        supplier.City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();
        supplier.State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim().ToUpperInvariant();
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

    private void ValidateSupplierInput(
        string name, string? email, string? contactName,
        string? phone, string? city, string? state)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }
        else if (name.Trim().Length > 160)
        {
            ModelState.AddModelError("name", "Name cannot exceed 160 characters.");
        }

        if (!string.IsNullOrWhiteSpace(email) && email.Trim().Length > 160)
        {
            ModelState.AddModelError("email", "Email cannot exceed 160 characters.");
        }

        if (!string.IsNullOrWhiteSpace(contactName) && contactName.Trim().Length > 160)
        {
            ModelState.AddModelError("contactName", "Contact name cannot exceed 160 characters.");
        }

        if (!string.IsNullOrWhiteSpace(phone) && phone.Trim().Length > 30)
        {
            ModelState.AddModelError("phone", "Phone cannot exceed 30 characters.");
        }

        if (!string.IsNullOrWhiteSpace(city) && city.Trim().Length > 120)
        {
            ModelState.AddModelError("city", "City cannot exceed 120 characters.");
        }

        if (!string.IsNullOrWhiteSpace(state) && state.Trim().Length > 2)
        {
            ModelState.AddModelError("state", "State must be at most 2 letters (UF).");
        }
    }
}
