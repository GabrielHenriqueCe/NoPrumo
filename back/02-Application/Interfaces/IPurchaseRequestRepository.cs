using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.Interfaces;

public interface IPurchaseRequestRepository
{
    Task<PagedResult<PurchaseRequest>> ListAsync(
        int page,
        int size,
        long? projectId,
        PurchaseRequestStatus? status,
        CancellationToken cancellationToken);

    Task<PurchaseRequest?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<PurchaseRequest?> GetForUpdateAsync(long id, CancellationToken cancellationToken);

    void Add(PurchaseRequest purchaseRequest);

    void AddMovement(StockMovement stockMovement);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
