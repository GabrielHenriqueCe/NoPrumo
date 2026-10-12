using NoPrumo.Application.DTOs;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;

namespace NoPrumo.Application.Services;

public sealed class DashboardService(
    IDashboardRepository dashboardRepository,
    IStockMovementService stockMovementService,
    TimeProvider timeProvider) : IDashboardService
{
    public async Task<DashboardDto> GetAsync(IReadOnlySet<string> permissions, CancellationToken cancellationToken)
    {
        var today = timeProvider.Today();

        int? activeProjects = null, lateProjects = null, lateStages = null;
        if (permissions.Contains("view_projects"))
        {
            var underway = (await dashboardRepository.ListActiveProjectsWithStagesAsync(cancellationToken))
                .Where(project => project.IsUnderway())
                .ToList();

            activeProjects = underway.Count;
            lateProjects = underway.Count(project => project.IsLate(today));
            lateStages = underway.Sum(project => project.Stages.Count(stage => stage.IsLate(today)));
        }

        int? stockItemsBelowMinimum = permissions.Contains("view_stock")
            ? (await stockMovementService.ListBelowMinimumAsync(cancellationToken)).Count
            : null;

        int? trainingsNeedingAttention = null, unsignedPpeIssues = null;
        if (permissions.Contains("manage_safety"))
        {
            var employees = await dashboardRepository.ListActiveEmployeesWithTrainingsAsync(cancellationToken);
            trainingsNeedingAttention = employees.Sum(employee =>
                employee.LatestTrainings().Count(training => training.NeedsAttentionOn(today)));

            unsignedPpeIssues = await dashboardRepository.CountUnsignedPpeIssuesAsync(cancellationToken);
        }

        return new DashboardDto(
            activeProjects,
            lateProjects,
            lateStages,
            stockItemsBelowMinimum,
            trainingsNeedingAttention,
            unsignedPpeIssues);
    }
}
