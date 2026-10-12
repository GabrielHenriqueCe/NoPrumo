using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Tests.Fakes;

public sealed class FakeDashboardRepository : IDashboardRepository
{
    public List<Project> Projects { get; } = [];
    public List<Employee> Employees { get; } = [];
    public int UnsignedPpeIssues { get; set; }

    public Task<IReadOnlyList<Project>> ListActiveProjectsWithStagesAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Project>>(Projects);

    public Task<IReadOnlyList<Employee>> ListActiveEmployeesWithTrainingsAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Employee>>(Employees);

    public Task<int> CountUnsignedPpeIssuesAsync(CancellationToken cancellationToken) =>
        Task.FromResult(UnsignedPpeIssues);
}
