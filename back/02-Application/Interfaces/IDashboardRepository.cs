using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IDashboardRepository
{
    // Obras ativas no cadastro, com Stages carregado.
    Task<IReadOnlyList<Project>> ListActiveProjectsWithStagesAsync(CancellationToken cancellationToken);

    // Funcionários ativos, com EmployeeTrainings carregado.
    Task<IReadOnlyList<Employee>> ListActiveEmployeesWithTrainingsAsync(CancellationToken cancellationToken);

    Task<int> CountUnsignedPpeIssuesAsync(CancellationToken cancellationToken);
}
