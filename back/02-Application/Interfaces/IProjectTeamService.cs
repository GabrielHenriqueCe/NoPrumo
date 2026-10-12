using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;

namespace NoPrumo.Application.Interfaces;

public interface IProjectTeamService
{
    Task<IReadOnlyList<ProjectTeamDto>?> ListAsync(long projectId, CancellationToken cancellationToken);

    Task<IReadOnlyList<TeamOptionDto>> ListTeamOptionsAsync(CancellationToken cancellationToken);

    Task<SaveProjectTeamResult?> AllocateAsync(long projectId, AllocateTeamRequest request, CancellationToken cancellationToken);

    Task<SaveProjectTeamResult?> EndAsync(long projectId, long teamId, EndTeamAllocationRequest request, CancellationToken cancellationToken);
}
