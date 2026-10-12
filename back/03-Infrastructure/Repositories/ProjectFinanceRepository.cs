using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Results;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class ProjectFinanceRepository(AppDbContext appDbContext) : IProjectFinanceRepository
{
    public Task<Project?> GetWithStagesAndAmendmentsAsync(long projectId, CancellationToken cancellationToken) =>
        appDbContext.Project
            .AsNoTracking()
            .Include(project => project.Stages)
            .Include(project => project.Amendments)
            .FirstOrDefaultAsync(project => project.Id == projectId, cancellationToken);

    public async Task<IReadOnlyList<Project>> ListActiveWithStagesAndAmendmentsAsync(CancellationToken cancellationToken) =>
        await appDbContext.Project
            .AsNoTracking()
            .Include(project => project.Stages)
            .Include(project => project.Amendments)
            .Where(project => project.DeletedAt == null)
            .OrderBy(project => project.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<DailyAmount>> ListMaterialCostByDayAsync(long projectId, CancellationToken cancellationToken)
    {
        var days = await MaterialEntries()
            .Where(movement => movement.ProjectId == projectId)
            .GroupBy(movement => movement.Date.Date)
            .Select(day => new { Day = day.Key, Amount = day.Sum(movement => movement.Quantity * movement.UnitCost) })
            .ToListAsync(cancellationToken);

        return days.Select(day => new DailyAmount(DateOnly.FromDateTime(day.Day), day.Amount)).ToList();
    }

    public async Task<IReadOnlyList<DailyAmount>> ListLaborCostByDayAsync(long projectId, CancellationToken cancellationToken)
    {
        var days = await appDbContext.TimeEntry
            .AsNoTracking()
            .Where(timeEntry => timeEntry.ProjectId == projectId)
            .GroupBy(timeEntry => timeEntry.Date)
            .Select(day => new { Day = day.Key, Amount = day.Sum(timeEntry => timeEntry.Cost) })
            .ToListAsync(cancellationToken);

        return days.Select(day => new DailyAmount(day.Day, day.Amount)).ToList();
    }

    public async Task<IReadOnlyDictionary<long, decimal>> SumMaterialCostByProjectAsync(CancellationToken cancellationToken) =>
        await MaterialEntries()
            .GroupBy(movement => movement.ProjectId!.Value)
            .Select(project => new { ProjectId = project.Key, Amount = project.Sum(movement => movement.Quantity * movement.UnitCost) })
            .ToDictionaryAsync(project => project.ProjectId, project => project.Amount, cancellationToken);

    public async Task<IReadOnlyDictionary<long, decimal>> SumLaborCostByProjectAsync(CancellationToken cancellationToken) =>
        await appDbContext.TimeEntry
            .AsNoTracking()
            .GroupBy(timeEntry => timeEntry.ProjectId)
            .Select(project => new { ProjectId = project.Key, Amount = project.Sum(timeEntry => timeEntry.Cost) })
            .ToDictionaryAsync(project => project.ProjectId, project => project.Amount, cancellationToken);

    // Mesmo critério do ListMaterialCostAsync do estoque: só entrada, na obra, de material de consumo.
    private IQueryable<StockMovement> MaterialEntries() =>
        appDbContext.StockMovement
            .AsNoTracking()
            .Where(movement =>
                movement.ProjectId != null
                && movement.Type == StockMovementType.Entry
                && movement.StockItem.StockGroup.StockCategory.TracksProjectBalance);
}
