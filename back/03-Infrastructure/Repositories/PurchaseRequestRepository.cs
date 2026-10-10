using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class PurchaseRequestRepository(AppDbContext appDbContext) : IPurchaseRequestRepository
{
    public async Task<PagedResult<PurchaseRequest>> ListAsync(
        int page,
        int size,
        long? projectId,
        PurchaseRequestStatus? status,
        CancellationToken cancellationToken)
    {
        var query = appDbContext.PurchaseRequest.AsNoTracking();

        if (projectId.HasValue)
        {
            query = query.Where(request => request.ProjectId == projectId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(request => request.Status == status.Value);
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(request => request.Project)
            .Include(request => request.RequestedByUser)
            .Include(request => request.DecidedByUser)
            .Include(request => request.Items)
                .ThenInclude(item => item.StockItem)
            .OrderByDescending(request => request.CreatedAt)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));

        return new PagedResult<PurchaseRequest>(items, page, size, total, totalPages);
    }

    public Task<PurchaseRequest?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        appDbContext.PurchaseRequest
            .AsNoTracking()
            .Include(request => request.Project)
            .Include(request => request.RequestedByUser)
            .Include(request => request.DecidedByUser)
            .Include(request => request.Items)
                .ThenInclude(item => item.StockItem)
            .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);

    public Task<PurchaseRequest?> GetForUpdateAsync(long id, CancellationToken cancellationToken) =>
        appDbContext.PurchaseRequest
            .Include(request => request.Items)
                .ThenInclude(item => item.StockItem)
            .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);

    public void Add(PurchaseRequest purchaseRequest) =>
        appDbContext.PurchaseRequest.Add(purchaseRequest);

    public void AddMovement(StockMovement stockMovement) =>
        appDbContext.StockMovement.Add(stockMovement);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        appDbContext.SaveChangesAsync(cancellationToken);
}
