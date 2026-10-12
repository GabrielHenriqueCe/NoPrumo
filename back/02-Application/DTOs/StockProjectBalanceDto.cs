namespace NoPrumo.Application.DTOs;

public sealed record StockProjectBalanceDto(
    long StockItemId,
    string Name,
    string Unit,
    decimal MinQuantity,
    decimal Balance);