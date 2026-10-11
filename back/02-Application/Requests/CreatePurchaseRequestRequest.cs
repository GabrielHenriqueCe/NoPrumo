namespace NoPrumo.Application.Requests;

public sealed record CreatePurchaseRequestRequest(
    long ProjectId,
    DateOnly? NeededByDate,
    string? Notes,
    IReadOnlyList<CreatePurchaseRequestItemRequest> Items);
