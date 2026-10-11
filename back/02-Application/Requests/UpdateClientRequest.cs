namespace NoPrumo.Application.Requests;

public sealed record UpdateClientRequest(
    string Name,
    string? PersonType,
    string? Document,
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
