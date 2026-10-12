using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Results;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Tests.Fakes;

public sealed class FakeProjectFinanceRepository : IProjectFinanceRepository
{
    public List<Project> Projects { get; } = [];
    public List<DailyAmount> MaterialByDay { get; } = [];
    public List<DailyAmount> LaborByDay { get; } = [];
    public Dictionary<long, decimal> MaterialByProject { get; } = [];
    public Dictionary<long, decimal> LaborByProject { get; } = [];

    public Task<Project?> GetWithStagesAndAmendmentsAsync(long projectId, CancellationToken cancellationToken) =>
        Task.FromResult(Projects.FirstOrDefault(project => project.Id == projectId));

    public Task<IReadOnlyList<Project>> ListActiveWithStagesAndAmendmentsAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Project>>(Projects);

    public Task<IReadOnlyList<DailyAmount>> ListMaterialCostByDayAsync(long projectId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<DailyAmount>>(MaterialByDay);

    public Task<IReadOnlyList<DailyAmount>> ListLaborCostByDayAsync(long projectId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<DailyAmount>>(LaborByDay);

    public Task<IReadOnlyDictionary<long, decimal>> SumMaterialCostByProjectAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<long, decimal>>(MaterialByProject);

    public Task<IReadOnlyDictionary<long, decimal>> SumLaborCostByProjectAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyDictionary<long, decimal>>(LaborByProject);
}
