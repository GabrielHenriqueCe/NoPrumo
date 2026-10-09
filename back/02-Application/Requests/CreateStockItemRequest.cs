namespace NoPrumo.Application.Requests;

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