using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IProjectTeamRepository
{
    Task<bool> ProjectExistsAsync(long projectId, CancellationToken cancellationToken);

    Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken);

    Task<Team?> GetTeamAsync(long teamId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Team>> ListActiveTeamsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<TeamProject>> ListByProjectAsync(long projectId, CancellationToken cancellationToken);

    Task<TeamProject?> GetAsync(long projectId, long teamId, DateOnly startDate, CancellationToken cancellationToken);

    Task<IReadOnlyList<TeamProject>> ListPeriodsAsync(long projectId, long teamId, CancellationToken cancellationToken);

    Task<TeamProject?> GetOpenForUpdateAsync(long projectId, long teamId, CancellationToken cancellationToken);

    void Add(TeamProject allocation);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
