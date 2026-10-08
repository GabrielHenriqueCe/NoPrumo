namespace NoPrumo.Application.DTOs.Requests;

public sealed record CreateSupplierRequest(
    string Name,
    string? Document,
    string? ContactName,
    string? Phone,
    string? Email,
    string? City,
    string? State,
    string? Notes);
