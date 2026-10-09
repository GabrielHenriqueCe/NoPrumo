namespace NoPrumo.Application.DTOs;

/// <summary>
/// O mesmo item, mas para quem tem view_finance (admin, engineer, purchasing).
/// Único DTO com ReferencePrice — nunca um campo anulável no DTO de cima.
/// </summary>
public sealed record StockItemWithPriceDto(
    long Id,
    long StockGroupId,
    string StockGroupName,
    string? Code,
    string Name,
    string Unit,
    decimal MinQuantity,
    decimal ReferencePrice,
    string? Ca,
    DateOnly? CaExpiryDate,
    bool Active);