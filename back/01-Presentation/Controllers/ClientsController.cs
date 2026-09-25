using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Gestão de clientes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_projects")]
public sealed class ClientsController(AppDbContext db) : ControllerBase
{
    private const int MaxPageSize = 100;

    [HttpGet]
    public async Task<ActionResult<PagedResult<ClientDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var query = db.Client
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(c =>
                c.Name.Contains(term) ||
                (c.DocumentMasked != null && c.DocumentMasked.Contains(term)) ||
                (c.Email != null && c.Email.Contains(term)) ||
                (c.ContactName != null && c.ContactName.Contains(term)));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<ClientDto>(
            items.Select(ClientDto.FromEntity).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ClientDto>> GetById(long id)
    {
        var client = await db.Client
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (client is null) return NotFound();

        return Ok(ClientDto.FromEntity(client));
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(CreateClientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }

        var personType = string.IsNullOrWhiteSpace(request.PersonType) ? "individual" : request.PersonType.Trim().ToLowerInvariant();
        if (personType != "individual" && personType != "company")
        {
            // Accept both individual/company and pf/pj for flexibility
            if (personType == "pf") personType = "individual";
            else if (personType == "pj") personType = "company";
            else ModelState.AddModelError("personType", "Person type must be individual (PF) or company (PJ).");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var client = new Client
        {
            Name = request.Name.Trim(),
            PersonType = personType,
            DocumentMasked = string.IsNullOrWhiteSpace(request.DocumentMasked) ? null : request.DocumentMasked.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            ContactName = string.IsNullOrWhiteSpace(request.ContactName) ? null : request.ContactName.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Mobile = string.IsNullOrWhiteSpace(request.Mobile) ? null : request.Mobile.Trim(),
            Street = string.IsNullOrWhiteSpace(request.Street) ? null : request.Street.Trim(),
            Number = string.IsNullOrWhiteSpace(request.Number) ? null : request.Number.Trim(),
            Complement = string.IsNullOrWhiteSpace(request.Complement) ? null : request.Complement.Trim(),
            District = string.IsNullOrWhiteSpace(request.District) ? null : request.District.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim(),
            PostalCode = string.IsNullOrWhiteSpace(request.PostalCode) ? null : request.PostalCode.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        db.Client.Add(client);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = client.Id }, ClientDto.FromEntity(client));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ClientDto>> Update(long id, UpdateClientRequest request)
    {
        var client = await db.Client.SingleOrDefaultAsync(c => c.Id == id);
        if (client is null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }

        var personType = string.IsNullOrWhiteSpace(request.PersonType) ? "individual" : request.PersonType.Trim().ToLowerInvariant();
        if (personType != "individual" && personType != "company")
        {
            if (personType == "pf") personType = "individual";
            else if (personType == "pj") personType = "company";
            else ModelState.AddModelError("personType", "Person type must be individual (PF) or company (PJ).");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        client.Name = request.Name.Trim();
        client.PersonType = personType;
        client.DocumentMasked = string.IsNullOrWhiteSpace(request.DocumentMasked) ? null : request.DocumentMasked.Trim();
        client.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        client.ContactName = string.IsNullOrWhiteSpace(request.ContactName) ? null : request.ContactName.Trim();
        client.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        client.Mobile = string.IsNullOrWhiteSpace(request.Mobile) ? null : request.Mobile.Trim();
        client.Street = string.IsNullOrWhiteSpace(request.Street) ? null : request.Street.Trim();
        client.Number = string.IsNullOrWhiteSpace(request.Number) ? null : request.Number.Trim();
        client.Complement = string.IsNullOrWhiteSpace(request.Complement) ? null : request.Complement.Trim();
        client.District = string.IsNullOrWhiteSpace(request.District) ? null : request.District.Trim();
        client.City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();
        client.State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim();
        client.PostalCode = string.IsNullOrWhiteSpace(request.PostalCode) ? null : request.PostalCode.Trim();
        client.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        client.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Ok(ClientDto.FromEntity(client));
    }

    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        var client = await db.Client.SingleOrDefaultAsync(c => c.Id == id);
        if (client is null) return NotFound();

        client.DeletedAt = null;
        client.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var client = await db.Client.SingleOrDefaultAsync(c => c.Id == id);
        if (client is null) return NotFound();

        client.DeletedAt = DateTime.UtcNow;
        client.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return NoContent();
    }
}
