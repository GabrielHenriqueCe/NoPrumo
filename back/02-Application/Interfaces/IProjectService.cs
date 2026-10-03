using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Projects;
using NoPrumo.Application.Requests.Projects;
using NoPrumo.Application.Results.Projects;

namespace NoPrumo.Application.Interfaces;

public interface IProjectService
{
    Task<PagedResult<ProjectDto>> ListAsync(int page, int size, string? search, bool canViewFinance, CancellationToken cancellationToken);

    Task<ProjectDto?> GetByIdAsync(long id, bool canViewFinance, CancellationToken cancellationToken);

    Task<SaveProjectResult> CreateAsync(SaveProjectRequest request, bool canViewFinance, CancellationToken cancellationToken);

    Task<SaveProjectResult?> UpdateAsync(long id, SaveProjectRequest request, bool canViewFinance, CancellationToken cancellationToken);
}