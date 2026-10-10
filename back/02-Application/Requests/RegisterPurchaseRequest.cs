namespace NoPrumo.Application.Requests;

public sealed record RegisterPurchaseRequest(
    long? SupplierId,
    string? InvoiceNumber,
    IReadOnlyList<RegisterPurchaseItemRequest> Items);
