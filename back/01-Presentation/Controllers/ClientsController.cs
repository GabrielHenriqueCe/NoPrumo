using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_projects")]
public sealed class ClientsController(AppDbContext appDbContext, DocumentProcessor documentProcessor) : ControllerBase
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

        var query = appDbContext.Client
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(client =>
                client.Name.Contains(term) ||
                (client.DocumentMasked != null && client.DocumentMasked.Contains(term)) ||
                (client.Email != null && client.Email.Contains(term)) ||
                (client.ContactName != null && client.ContactName.Contains(term)));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(client => client.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<ClientDto>(
            items.Select(MapToDto).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ClientDto>> GetById(long id)
    {
        var client = await appDbContext.Client
            .AsNoTracking()
            .FirstOrDefaultAsync(client => client.Id == id);

        if (client is null) return NotFound();

        return Ok(MapToDto(client));
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(CreateClientRequest request)
    {
        ValidateClientInput(request.Name, request.PersonType, request.Email, request.ContactName,
            request.Phone, request.Mobile, request.Street, request.Number, request.Complement,
            request.District, request.City, request.State, request.PostalCode);

        ProcessedDocument? processedDocument = null;
        if (!string.IsNullOrWhiteSpace(request.Document))
        {
            if (!DocumentProcessor.IsValid(request.Document))
            {
                ModelState.AddModelError("document", "Invalid CPF or CNPJ format.");
            }
            else
            {
                processedDocument = documentProcessor.Process(request.Document);
                if (processedDocument.Hash != null && await appDbContext.Client.AnyAsync(client => client.DocumentHash == processedDocument.Hash && client.DeletedAt == null))
                {
                    ModelState.AddModelError("document", "This document is already registered.");
                }
            }
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var personType = Enum.TryParse<PersonType>(request.PersonType, true, out var parsedType)
            ? parsedType
            : PersonType.Company;

        var client = new Client
        {
            Name = request.Name.Trim(),
            PersonType = personType,
            DocumentEncrypted = processedDocument?.Encrypted,
            DocumentHash = processedDocument?.Hash,
            DocumentMasked = processedDocument?.Masked,
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            ContactName = string.IsNullOrWhiteSpace(request.ContactName) ? null : request.ContactName.Trim(),
            Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            Mobile = string.IsNullOrWhiteSpace(request.Mobile) ? null : request.Mobile.Trim(),
            Street = string.IsNullOrWhiteSpace(request.Street) ? null : request.Street.Trim(),
            Number = string.IsNullOrWhiteSpace(request.Number) ? null : request.Number.Trim(),
            Complement = string.IsNullOrWhiteSpace(request.Complement) ? null : request.Complement.Trim(),
            District = string.IsNullOrWhiteSpace(request.District) ? null : request.District.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim().ToUpperInvariant(),
            PostalCode = string.IsNullOrWhiteSpace(request.PostalCode) ? null : request.PostalCode.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
        };

        appDbContext.Client.Add(client);
        await appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = client.Id }, MapToDto(client));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ClientDto>> Update(long id, UpdateClientRequest request)
    {
        var client = await appDbContext.Client.SingleOrDefaultAsync(client => client.Id == id);
        if (client is null) return NotFound();

        ValidateClientInput(request.Name, request.PersonType, request.Email, request.ContactName,
            request.Phone, request.Mobile, request.Street, request.Number, request.Complement,
            request.District, request.City, request.State, request.PostalCode);

        ProcessedDocument? processedDocument = null;
        bool hasNewDocument = !string.IsNullOrWhiteSpace(request.Document);

        if (hasNewDocument)
        {
            if (!DocumentProcessor.IsValid(request.Document))
            {
                ModelState.AddModelError("document", "Invalid CPF or CNPJ format.");
            }
            else
            {
                processedDocument = documentProcessor.Process(request.Document);
                if (processedDocument.Hash != null && await appDbContext.Client.AnyAsync(client => client.DocumentHash == processedDocument.Hash && client.Id != id && client.DeletedAt == null))
                {
                    ModelState.AddModelError("document", "This document is already registered.");
                }
            }
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var personType = Enum.TryParse<PersonType>(request.PersonType, true, out var parsedType)
            ? parsedType
            : PersonType.Company;

        client.Name = request.Name.Trim();
        client.PersonType = personType;

        if (hasNewDocument && processedDocument != null)
        {
            client.DocumentEncrypted = processedDocument.Encrypted;
            client.DocumentHash = processedDocument.Hash;
            client.DocumentMasked = processedDocument.Masked;
        }

        client.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        client.ContactName = string.IsNullOrWhiteSpace(request.ContactName) ? null : request.ContactName.Trim();
        client.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
        client.Mobile = string.IsNullOrWhiteSpace(request.Mobile) ? null : request.Mobile.Trim();
        client.Street = string.IsNullOrWhiteSpace(request.Street) ? null : request.Street.Trim();
        client.Number = string.IsNullOrWhiteSpace(request.Number) ? null : request.Number.Trim();
        client.Complement = string.IsNullOrWhiteSpace(request.Complement) ? null : request.Complement.Trim();
        client.District = string.IsNullOrWhiteSpace(request.District) ? null : request.District.Trim();
        client.City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim();
        client.State = string.IsNullOrWhiteSpace(request.State) ? null : request.State.Trim().ToUpperInvariant();
        client.PostalCode = string.IsNullOrWhiteSpace(request.PostalCode) ? null : request.PostalCode.Trim();
        client.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

        await appDbContext.SaveChangesAsync();

        return Ok(MapToDto(client));
    }

    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        var client = await appDbContext.Client.SingleOrDefaultAsync(client => client.Id == id);
        if (client is null) return NotFound();

        client.DeletedAt = null;
        await appDbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var client = await appDbContext.Client.SingleOrDefaultAsync(client => client.Id == id);
        if (client is null) return NotFound();

        client.DeletedAt = DateTime.UtcNow;
        await appDbContext.SaveChangesAsync();

        return NoContent();
    }

    private static ClientDto MapToDto(Client client) => new(
        client.Id,
        client.Name,
        client.PersonType.ToString().ToLowerInvariant(),
        client.DocumentMasked,
        client.Email,
        client.ContactName,
        client.Phone,
        client.Mobile,
        client.Street,
        client.Number,
        client.Complement,
        client.District,
        client.City,
        client.State,
        client.PostalCode,
        client.Notes,
        client.DeletedAt == null);

    private void ValidateClientInput(
        string name, string? personType, string? email, string? contactName,
        string? phone, string? mobile, string? street, string? number, string? complement,
        string? district, string? city, string? state, string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }
        else if (name.Trim().Length > 160)
        {
            ModelState.AddModelError("name", "Name cannot exceed 160 characters.");
        }

        if (!string.IsNullOrWhiteSpace(personType))
        {
            var pType = personType.Trim().ToLowerInvariant();
            if (pType != "company" && pType != "individual")
            {
                ModelState.AddModelError("personType", "Person type must be company or individual.");
            }
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

        if (!string.IsNullOrWhiteSpace(mobile) && mobile.Trim().Length > 30)
        {
            ModelState.AddModelError("mobile", "Mobile cannot exceed 30 characters.");
        }

        if (!string.IsNullOrWhiteSpace(street) && street.Trim().Length > 255)
        {
            ModelState.AddModelError("street", "Street cannot exceed 255 characters.");
        }

        if (!string.IsNullOrWhiteSpace(number) && number.Trim().Length > 20)
        {
            ModelState.AddModelError("number", "Number cannot exceed 20 characters.");
        }

        if (!string.IsNullOrWhiteSpace(complement) && complement.Trim().Length > 100)
        {
            ModelState.AddModelError("complement", "Complement cannot exceed 100 characters.");
        }

        if (!string.IsNullOrWhiteSpace(district) && district.Trim().Length > 100)
        {
            ModelState.AddModelError("district", "District cannot exceed 100 characters.");
        }

        if (!string.IsNullOrWhiteSpace(city) && city.Trim().Length > 120)
        {
            ModelState.AddModelError("city", "City cannot exceed 120 characters.");
        }

        if (!string.IsNullOrWhiteSpace(state) && state.Trim().Length > 2)
        {
            ModelState.AddModelError("state", "State must be at most 2 letters (UF).");
        }

        if (!string.IsNullOrWhiteSpace(postalCode) && postalCode.Trim().Length > 10)
        {
            ModelState.AddModelError("postalCode", "Postal code cannot exceed 10 characters.");
        }
    }
}
