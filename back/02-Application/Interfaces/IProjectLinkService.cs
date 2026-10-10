using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Responses;

namespace NoPrumo.Application.Interfaces;

public interface IProjectLinkService
{
    Task<CreateProjectLinkResponse?> CreateAsync(long projectId, CreateProjectLinkRequest request, long? createdByUserId, CancellationToken cancellationToken);

    Task<bool> RevokeAsync(long id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ProjectLinkDto>> ListByProjectIdAsync(long projectId, CancellationToken cancellationToken);

    Task<PortalProjectDto?> GetPortalProjectAsync(string token, CancellationToken cancellationToken);
}
