using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;

namespace NoPrumo.Tests.Fakes;

// Só o que o painel usa; o resto falha alto se algum teste chamar sem querer.
public sealed class FakeStockMovementService : IStockMovementService
{
    public List<StockBelowMinimumDto> BelowMinimum { get; } = [];

    public Task<IReadOnlyList<StockBelowMinimumDto>> ListBelowMinimumAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<StockBelowMinimumDto>>(BelowMinimum);

    public Task<PagedResult<StockMovementDto>> ListAsync(int page, int size, long? stockItemId, long? projectId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<StockBalanceDto?> GetBalanceAsync(long stockItemId, long? projectId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<IReadOnlyList<StockProjectBalanceDto>?> GetProjectBalanceAsync(long projectId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<MaterialCostDto?> GetMaterialCostAsync(long projectId, DateTime? from, DateTime? to, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<SaveStockMovementResult> CreateAsync(CreateStockMovementRequest request, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
