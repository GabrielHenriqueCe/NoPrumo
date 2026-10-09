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
    bool Active);