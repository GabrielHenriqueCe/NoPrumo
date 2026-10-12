namespace NoPrumo.Application.DTOs;

public sealed record MaterialCostItemDto(
    long StockItemId,
    string Name,
    string Unit,
    decimal Quantity,
    decimal Total);

public sealed record MaterialCostDto(
    long ProjectId,
    decimal Total,
    IReadOnlyList<MaterialCostItemDto> Items);