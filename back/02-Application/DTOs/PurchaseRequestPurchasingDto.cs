using System.Diagnostics.CodeAnalysis;

namespace NoPrumo.Application.DTOs;

public sealed record PurchaseRequestPurchasingDto : PurchaseRequestDto
{
    [SetsRequiredMembers]
    public PurchaseRequestPurchasingDto(PurchaseRequestDto request, bool canManagePurchases) : base(request)
    {
        CanManagePurchases = canManagePurchases;
    }

    public bool CanManagePurchases { get; init; }
}
