using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class PpeIssueRepository(AppDbContext appDbContext) : IPpeIssueRepository
{
    public async Task<PagedResult<PpeIssue>> ListAsync(int page, int size, string? search, long? projectId, long? employeeId, CancellationToken cancellationToken)
    {
        var query = appDbContext.PpeIssue.AsNoTracking();

        if (projectId is not null)
        {
            query = query.Where(issue => issue.ProjectId == projectId);
        }

        if (employeeId is not null)
        {
            query = query.Where(issue => issue.EmployeeId == employeeId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(issue =>
                issue.Employee.Name.Contains(term) ||
                (issue.Employee.RegistrationNumber != null && issue.Employee.RegistrationNumber.Contains(term)) ||
                issue.Items.Any(item => item.ItemNameSnapshot.Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);

        var issues = await WithDetails(query)
            .OrderByDescending(issue => issue.Date)
            .ThenByDescending(issue => issue.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));

        return new PagedResult<PpeIssue>(issues, page, size, total, totalPages);
    }

    public Task<PpeIssue?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        WithDetails(appDbContext.PpeIssue.AsNoTracking())
            .FirstOrDefaultAsync(issue => issue.Id == id, cancellationToken);

    public Task<PpeIssue?> GetForUpdateAsync(long id, CancellationToken cancellationToken) =>
        appDbContext.PpeIssue
            .Include(issue => issue.Employee)
            .Include(issue => issue.Items)
            .FirstOrDefaultAsync(issue => issue.Id == id, cancellationToken);

    public Task<Employee?> GetEmployeeAsync(long employeeId, CancellationToken cancellationToken) =>
        appDbContext.Employees.AsNoTracking().FirstOrDefaultAsync(employee => employee.Id == employeeId, cancellationToken);

    public Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken) =>
        appDbContext.Project.AnyAsync(project => project.Id == projectId && project.DeletedAt == null, cancellationToken);

    public async Task<IReadOnlyList<StockItem>> ListStockItemsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken) =>
        await appDbContext.StockItem
            .AsNoTracking()
            .Include(stockItem => stockItem.StockGroup)
                .ThenInclude(stockGroup => stockGroup.StockCategory)
            .Where(stockItem => ids.Contains(stockItem.Id))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StockItem>> ListActiveStockItemsAsync(CancellationToken cancellationToken) =>
        await appDbContext.StockItem
            .AsNoTracking()
            .Include(stockItem => stockItem.StockGroup)
                .ThenInclude(stockGroup => stockGroup.StockCategory)
            .Where(stockItem => stockItem.Active == true)
            .OrderBy(stockItem => stockItem.Name)
            .ThenBy(stockItem => stockItem.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Employee>> ListActiveEmployeesAsync(CancellationToken cancellationToken) =>
        await appDbContext.Employees
            .AsNoTracking()
            .Where(employee => employee.Active == true)
            .OrderBy(employee => employee.Name)
            .ThenBy(employee => employee.Id)
            .ToListAsync(cancellationToken);

    // A ficha e as saídas de estoque entram juntas ou nenhuma entra. São dois
    // SaveChanges porque a saída leva o Id da ficha (SourceId).
    public async Task AddWithStockExitsAsync(PpeIssue issue, DateTime now, long? recordedBy, CancellationToken cancellationToken)
    {
        await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellationToken);

        appDbContext.PpeIssue.Add(issue);
        await appDbContext.SaveChangesAsync(cancellationToken);

        appDbContext.StockMovement.AddRange(issue.StockExits(now, recordedBy));
        await appDbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        appDbContext.SaveChangesAsync(cancellationToken);

    private static IQueryable<PpeIssue> WithDetails(IQueryable<PpeIssue> query) =>
        query
            .Include(issue => issue.Employee)
            .Include(issue => issue.Project)
            .Include(issue => issue.IssuedBy)
            .Include(issue => issue.Items)
            .AsSplitQuery();
}
