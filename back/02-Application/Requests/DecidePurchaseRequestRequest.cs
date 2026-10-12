namespace NoPrumo.Application.Requests;

public sealed record DecidePurchaseRequestRequest(
    bool Approved,
    string? RejectionReason);
