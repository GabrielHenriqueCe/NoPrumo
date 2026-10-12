using NoPrumo.Application.DTOs;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Results;

namespace NoPrumo.Application.Services;

public sealed class ProjectFinanceService(IProjectFinanceRepository projectFinanceRepository, TimeProvider timeProvider) : IProjectFinanceService
{
    public async Task<ProjectFinancialsDto?> GetAsync(long projectId, CancellationToken cancellationToken)
    {
        var project = await projectFinanceRepository.GetWithStagesAndAmendmentsAsync(projectId, cancellationToken);
        if (project is null)
        {
            return null;
        }

        var materialByDay = await projectFinanceRepository.ListMaterialCostByDayAsync(projectId, cancellationToken);
        var laborByDay = await projectFinanceRepository.ListLaborCostByDayAsync(projectId, cancellationToken);

        var materialCost = materialByDay.Sum(day => day.Amount);
        var laborCost = laborByDay.Sum(day => day.Amount);
        var totalCost = materialCost + laborCost;
        var revisedContract = project.RevisedContractAmount();

        return new ProjectFinancialsDto(
            project.Id,
            project.ContractAmount,
            revisedContract - project.ContractAmount,
            revisedContract,
            materialCost,
            laborCost,
            totalCost,
            project.SpentPercentage(totalCost),
            project.ProgressPercentage(),
            project.IsCostAheadOfProgress(totalCost),
            project.Margin(totalCost),
            CostByDay(materialByDay, laborByDay));
    }

    public async Task<IReadOnlyList<ProjectClosingDto>> ListClosingAsync(CancellationToken cancellationToken)
    {
        var projects = await projectFinanceRepository.ListActiveWithStagesAndAmendmentsAsync(cancellationToken);
        var materialByProject = await projectFinanceRepository.SumMaterialCostByProjectAsync(cancellationToken);
        var laborByProject = await projectFinanceRepository.SumLaborCostByProjectAsync(cancellationToken);
        var today = timeProvider.Today();

        return projects
            .Select(project =>
            {
                var materialCost = materialByProject.GetValueOrDefault(project.Id);
                var laborCost = laborByProject.GetValueOrDefault(project.Id);
                var totalCost = materialCost + laborCost;

                return new ProjectClosingDto(
                    project.Id,
                    project.Code,
                    project.Name,
                    project.Status,
                    project.IsLate(today),
                    project.RevisedContractAmount(),
                    materialCost,
                    laborCost,
                    totalCost,
                    project.Margin(totalCost),
                    project.SpentPercentage(totalCost),
                    project.ProgressPercentage(),
                    project.IsCostAheadOfProgress(totalCost));
            })
            .ToList();
    }

    // Um ponto por dia com lançamento, em ordem, com o custo acumulado até aquele dia.
    private static List<DailyCostDto> CostByDay(IReadOnlyList<DailyAmount> materialByDay, IReadOnlyList<DailyAmount> laborByDay)
    {
        var material = materialByDay.ToDictionary(day => day.Date, day => day.Amount);
        var labor = laborByDay.ToDictionary(day => day.Date, day => day.Amount);
        var cumulativeCost = 0m;

        return material.Keys
            .Union(labor.Keys)
            .Order()
            .Select(date =>
            {
                var materialOfDay = material.GetValueOrDefault(date);
                var laborOfDay = labor.GetValueOrDefault(date);
                cumulativeCost += materialOfDay + laborOfDay;
                return new DailyCostDto(date, materialOfDay, laborOfDay, cumulativeCost);
            })
            .ToList();
    }
}
