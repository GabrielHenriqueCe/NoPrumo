using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;

namespace NoPrumo.Application.Interfaces;

public interface IStockMovementService
{
    Task<PagedResult<StockMovementDto>> ListAsync(int page, int size, long? stockItemId, long? projectId, CancellationToken cancellationToken);

    Task<StockBalanceDto?> GetBalanceAsync(long stockItemId, long? projectId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockBelowMinimumDto>> ListBelowMinimumAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<StockProjectBalanceDto>?> GetProjectBalanceAsync(long projectId, CancellationToken cancellationToken);

    Task<MaterialCostDto?> GetMaterialCostAsync(long projectId, DateTime? from, DateTime? to, CancellationToken cancellationToken);

    Task<SaveStockMovementResult> CreateAsync(CreateStockMovementRequest request, CancellationToken cancellationToken);
}