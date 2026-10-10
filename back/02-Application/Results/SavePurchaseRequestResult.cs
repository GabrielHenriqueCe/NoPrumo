using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Results;

public sealed record SavePurchaseRequestResult(
    PurchaseRequestDto? Request,
    IReadOnlyDictionary<string, string> Errors)
{
    public bool Succeeded => Errors.Count == 0 && Request is not null;
}
