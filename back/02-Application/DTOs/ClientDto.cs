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
    bool Active);
