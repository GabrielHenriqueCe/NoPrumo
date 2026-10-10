namespace NoPrumo.Application.DTOs;

public sealed record StockBelowMinimumDto(
    long StockItemId,
    string StockItemName,
    string Unit,
    decimal MinQuantity,
    decimal Balance);