using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// O item de estoque para quem NÃO tem view_finance (foreman, warehouse_keeper).
/// Sem ReferencePrice: a regra é "mestre enxerga quantidade, admin enxerga
/// dinheiro" — o campo não existe aqui, não é escondido na tela.
/// </summary>
public sealed record StockItemDto(
    long Id,
    long StockGroupId,
    string StockGroupName,
    string? Code,
    string Name,
    string Unit,
    decimal MinQuantity,
    string? Ca,
    DateOnly? CaExpiryDate,
    bool Active)
{
    /// <summary>Exige StockGroup carregado (.Include(i => i.StockGroup)).</summary>
    public static StockItemDto FromEntity(StockItem i) => new(
        i.Id, i.StockGroupId, i.StockGroup.Name, i.Code, i.Name, i.Unit,
        i.MinQuantity, i.Ca, i.CaExpiryDate, i.Active ?? true);
}

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
    bool Active)
{
    public static StockItemWithPriceDto FromEntity(StockItem i) => new(
        i.Id, i.StockGroupId, i.StockGroup.Name, i.Code, i.Name, i.Unit,
        i.MinQuantity, i.ReferencePrice, i.Ca, i.CaExpiryDate, i.Active ?? true);
}

// Quem só tem manage_stock nunca manda ReferencePrice — o campo nem existe
// aqui. Quem tem view_finance também manda pelo mesmo request; o controller
// decide se aplica o valor, olhando a claim de quem está autenticado.
public sealed record CreateStockItemRequest(
    long StockGroupId,
    string? Code,
    string Name,
    string Unit,
    decimal MinQuantity,
    string? Ca,
    DateOnly? CaExpiryDate,
    decimal? ReferencePrice);

public sealed record UpdateStockItemRequest(
    long StockGroupId,
    string? Code,
    string Name,
    string Unit,
    decimal MinQuantity,
    string? Ca,
    DateOnly? CaExpiryDate,
    decimal? ReferencePrice);