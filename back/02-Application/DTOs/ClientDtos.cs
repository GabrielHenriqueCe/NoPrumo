using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.DTOs;

public sealed record ClientDto(
    long Id,
    string Name,
    string PersonType,
    string? DocumentMasked,
    string? Email,
    string? ContactName,
    string? Phone,
    string? Mobile,
    string? Street,
    string? Number,
    string? Complement,
    string? District,
    string? City,
    string? State,
    string? PostalCode,
    string? Notes,
    bool Active)
{
    public static ClientDto FromEntity(Client client) => new(
        client.Id,
        client.Name,
        client.PersonType,
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
}

public sealed record CreateClientRequest(
    string Name,
    string PersonType,
    string? DocumentMasked,
    string? Email,
    string? ContactName,
    string? Phone,
    string? Mobile,
    string? Street,
    string? Number,
    string? Complement,
    string? District,
    string? City,
    string? State,
    string? PostalCode,
    string? Notes);

public sealed record UpdateClientRequest(
    string Name,
    string PersonType,
    string? DocumentMasked,
    string? Email,
    string? ContactName,
    string? Phone,
    string? Mobile,
    string? Street,
    string? Number,
    string? Complement,
    string? District,
    string? City,
    string? State,
    string? PostalCode,
    string? Notes);
