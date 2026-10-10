namespace NoPrumo.Application.Requests;

public sealed record CreatePurchaseRequestItemRequest(
    long StockItemId,
    decimal RequestedQuantity,
    string Unit,
    string? Notes);
