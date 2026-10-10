namespace NoPrumo.Application.DTOs;

/// <summary>
/// O saldo de um item. Para categorias com saldo por obra (consumable
/// material), ProjectId/ProjectName vêm preenchidos e o saldo é só daquela
/// obra; para as demais (tool), vêm nulos e o saldo é do item inteiro.
/// </summary>
public sealed record StockBalanceDto(
    long StockItemId,
    string StockItemName,
    string Unit,
    long? ProjectId,
    string? ProjectName,
    decimal Balance);