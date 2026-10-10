namespace NoPrumo.Application.Requests;

public sealed record RegisterPurchaseItemRequest(
    long PurchaseRequestItemId,
    decimal Quantity,
    decimal UnitCost,
    long? SupplierId,
    string? InvoiceNumber);
