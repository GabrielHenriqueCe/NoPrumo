using NoPrumo.Domain.Entities;

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
    bool Active)
{
    public static SupplierDto FromEntity(Supplier supplier) => new(
        supplier.Id,
        supplier.Name,
        supplier.DocumentMasked,
        supplier.ContactName,
        supplier.Phone,
        supplier.Email,
        supplier.City,
        supplier.State,
        supplier.Notes,
        supplier.Active ?? (supplier.DeletedAt == null));
}

public sealed record CreateSupplierRequest(
    string Name,
    string? Document,
    string? ContactName,
    string? Phone,
    string? Email,
    string? City,
    string? State,
    string? Notes);

public sealed record UpdateSupplierRequest(
    string Name,
    string? Document,
    string? ContactName,
    string? Phone,
    string? Email,
    string? City,
    string? State,
    string? Notes);
