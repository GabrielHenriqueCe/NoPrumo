using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class StockMovementRepository(AppDbContext appDbContext) : IStockMovementRepository
{
    public async Task<PagedResult<StockMovement>> ListAsync(int page, int size, long? stockItemId, long? projectId, CancellationToken cancellationToken)
    {
        var query = appDbContext.StockMovement.AsNoTracking();

        if (stockItemId is not null)
        {
            query = query.Where(movement => movement.StockItemId == stockItemId);
        }

        if (projectId is not null)
        {
            query = query.Where(movement => movement.ProjectId == projectId);
        }

        var total = await query.CountAsync(cancellationToken);

        var movements = await WithDetails(query)
            .OrderByDescending(movement => movement.Date)
            .ThenByDescending(movement => movement.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));

        return new PagedResult<StockMovement>(movements, page, size, total, totalPages);
    }

    public Task<StockMovement?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        WithDetails(appDbContext.StockMovement.AsNoTracking())
            .FirstOrDefaultAsync(movement => movement.Id == id, cancellationToken);

    public Task<StockItem?> GetStockItemWithCategoryAsync(long stockItemId, CancellationToken cancellationToken) =>
        appDbContext.StockItem
        .Include(stockItem => stockItem.StockGroup)
        .ThenInclude(stockGroup => stockGroup.StockCategory)
        .FirstOrDefaultAsync(stockItem => stockItem.Id == stockItemId, cancellationToken);

    public Task<Project?> GetProjectAsync(long projectId, CancellationToken cancellationToken) =>
        appDbContext.Project
        .FirstOrDefaultAsync(project => project.Id == projectId, cancellationToken);

    public Task<Employee?> GetEmployeeAsync(long employeeId, CancellationToken cancellationToken) =>
        appDbContext.Employees
        .FirstOrDefaultAsync(employee => employee.Id == employeeId, cancellationToken);

    public async Task<decimal> GetBalanceAsync(long stockItemId, long? projectId, CancellationToken cancellationToken)
    {
        var query = appDbContext.StockMovement.AsNoTracking().Where(movement => movement.StockItemId == stockItemId);

        if (projectId is not null)
        {
            query = query.Where(movement => movement.ProjectId == projectId);
        }

        return await query.SumAsync(movement => movement.Type == StockMovementType.Entry ? movement.Quantity : -movement.Quantity, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<long, decimal>> ListBalancesByItemAsync(long? projectId, CancellationToken cancellationToken)
    {
        var query = appDbContext.StockMovement.AsNoTracking();

        if (projectId is not null)
        {
            query = query.Where(movement => movement.ProjectId == projectId);
        }

        return await query
            .GroupBy(movement => movement.StockItemId)
            .Select(group => new
            {
                StockItemId = group.Key,
                Balance = group.Sum(movement => movement.Type == StockMovementType.Entry ? movement.Quantity : -movement.Quantity),
            })
            .ToDictionaryAsync(entry => entry.StockItemId, entry => entry.Balance, cancellationToken);
    }

    public async Task<IReadOnlyList<StockItem>> ListActiveStockItemsAsync(CancellationToken cancellationToken) =>
        await appDbContext.StockItem
            .AsNoTracking()
            .Where(stockItem => stockItem.Active == true)
            .OrderBy(stockItem => stockItem.Name)
            .ThenBy(stockItem => stockItem.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StockItem>> ListProjectBalanceItemsAsync(CancellationToken cancellationToken)
    {
        // Só leitura, então AsNoTracking. Só categorias com saldo por obra (consumable material)
        return await appDbContext.StockItem
            .AsNoTracking()
            .Where(item => item.Active == true && item.StockGroup.StockCategory.TracksProjectBalance)
            .OrderBy(item => item.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MaterialCostItemDto>> ListMaterialCostAsync(
        long projectId,
        DateTime? from,
        DateTime? toExclusive,
        CancellationToken cancellationToken)
    {
        // Só entradas na obra, de categoria com saldo por obra. O custo vem do UnitCost gravado na entrada
        var rows = await appDbContext.StockMovement
            .AsNoTracking()
            .Where(movement => movement.ProjectId == projectId
                && movement.Type == StockMovementType.Entry
                && movement.StockItem.StockGroup.StockCategory.TracksProjectBalance
                && (from == null || movement.Date >= from)
                && (toExclusive == null || movement.Date < toExclusive))
            .GroupBy(movement => new { movement.StockItemId, movement.StockItem.Name, movement.Unit })
            .Select(group => new
            {
                group.Key.StockItemId,
                group.Key.Name,
                group.Key.Unit,
                Quantity = group.Sum(movement => movement.Quantity),
                Total = group.Sum(movement => movement.Quantity * movement.UnitCost),
            })
            .OrderBy(row => row.Name)
            .ToListAsync(cancellationToken);

        return rows
            .Select(row => new MaterialCostItemDto(row.StockItemId, row.Name, row.Unit, row.Quantity, row.Total))
            .ToArray();
    }

    public async Task AddAsync(StockMovement movement, CancellationToken cancellationToken)
    {
        appDbContext.StockMovement.Add(movement);
        await appDbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<StockMovement> WithDetails(IQueryable<StockMovement> query) =>
        query
            .Include(movement => movement.StockItem)
            .Include(movement => movement.Project)
            .Include(movement => movement.Employee);
}