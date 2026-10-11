using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;

namespace NoPrumo.Application.Interfaces;

public interface IPurchaseRequestService
{
    Task<PagedResult<PurchaseRequestDto>> ListAsync(
        int page,
        int size,
        long? projectId,
        string? status,
        bool canManagePurchases,
        CancellationToken cancellationToken);

    Task<PurchaseRequestDto?> GetByIdAsync(
        long id,
        bool canManagePurchases,
        CancellationToken cancellationToken);

    Task<SavePurchaseRequestResult> CreateAsync(
        CreatePurchaseRequestRequest request,
        long? requestedByUserId,
        bool canManagePurchases,
        CancellationToken cancellationToken);

    Task<SavePurchaseRequestResult?> DecideAsync(
        long id,
        DecidePurchaseRequestRequest request,
        long decidedByUserId,
        bool canManagePurchases,
        CancellationToken cancellationToken);

    Task<SavePurchaseRequestResult?> RegisterPurchaseAsync(
        long id,
        RegisterPurchaseRequest request,
        long recordedByUserId,
        bool canManagePurchases,
        CancellationToken cancellationToken);
}
