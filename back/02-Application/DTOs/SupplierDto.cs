namespace NoPrumo.Application.DTOs;

public sealed record SupplierDto(
    long Id,
    string Name,
    string? DocumentMasked,
    string? ContactName,
    string? Phone,
    string? Email,
    string? City,
    string? State,
    string? Notes,
    bool Active);
