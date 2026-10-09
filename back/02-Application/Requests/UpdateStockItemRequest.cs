namespace NoPrumo.Application.Requests;

public sealed record UpdateStockItemRequest(
    long StockGroupId,
    string? Code,
    string Name,
    string Unit,
    decimal MinQuantity,
    string? Ca,
    DateOnly? CaExpiryDate,
    decimal? ReferencePrice);