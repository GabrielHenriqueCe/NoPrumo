using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IStockMovementRepository
{
    Task<PagedResult<StockMovement>> ListAsync(int page, int size, long? stockItemId, long? projectId, CancellationToken cancellationToken);

    Task<StockMovement?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<StockItem?> GetStockItemWithCategoryAsync(long stockItemId, CancellationToken cancellationToken);

    Task<Project?> GetProjectAsync(long projectId, CancellationToken cancellationToken);

    Task<Employee?> GetEmployeeAsync(long employeeId, CancellationToken cancellationToken);

    Task<decimal> GetBalanceAsync(long stockItemId, long? projectId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, decimal>> ListBalancesByItemAsync(long? projectId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockItem>> ListActiveStockItemsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<StockItem>> ListProjectBalanceItemsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<MaterialCostItemDto>> ListMaterialCostAsync(long projectId, DateTime? from, DateTime? toExclusive, CancellationToken cancellationToken);

    Task AddAsync(StockMovement movement, CancellationToken cancellationToken);
}