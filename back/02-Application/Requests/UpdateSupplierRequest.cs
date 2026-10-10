namespace NoPrumo.Application.Requests;

public sealed record UpdateSupplierRequest(
    string Name,
    string? Document,
    string? ContactName,
    string? Phone,
    string? Email,
    string? City,
    string? State,
    string? Notes);
