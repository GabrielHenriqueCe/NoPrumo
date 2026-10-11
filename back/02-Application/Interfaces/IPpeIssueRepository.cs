using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IPpeIssueRepository
{
    Task<PagedResult<PpeIssue>> ListAsync(int page, int size, string? search, long? projectId, long? employeeId, CancellationToken cancellationToken);

    Task<PpeIssue?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<PpeIssue?> GetForUpdateAsync(long id, CancellationToken cancellationToken);

    Task<Employee?> GetEmployeeAsync(long employeeId, CancellationToken cancellationToken);

    Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockItem>> ListStockItemsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken);

    Task<IReadOnlyList<StockItem>> ListActiveStockItemsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Employee>> ListActiveEmployeesAsync(CancellationToken cancellationToken);

    Task AddWithStockExitsAsync(PpeIssue issue, DateTime now, long? recordedBy, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
