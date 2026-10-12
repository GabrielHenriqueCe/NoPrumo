using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Interfaces;

public interface IProjectFinanceService
{
    // null = obra não existe
    Task<ProjectFinancialsDto?> GetAsync(long projectId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ProjectClosingDto>> ListClosingAsync(CancellationToken cancellationToken);
}
