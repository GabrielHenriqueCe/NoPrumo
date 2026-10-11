using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IProjectLinkRepository
{
    Task<ProjectLink?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken);

    Task<ProjectLink?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ProjectLink>> ListByProjectIdAsync(long projectId, CancellationToken cancellationToken);

    void Add(ProjectLink projectLink);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
