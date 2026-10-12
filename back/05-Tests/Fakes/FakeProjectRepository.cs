using NoPrumo.Application.Access;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Tests.Fakes;

// Imita o filtro do ProjectRepository: quem não vê tudo só acha obra que supervisiona.
public sealed class FakeProjectRepository : IProjectRepository
{
    public List<Project> Projects { get; } = [];

    public Task<Project?> GetByIdAsync(long id, ProjectAccess access, CancellationToken cancellationToken) =>
        Task.FromResult(Projects.FirstOrDefault(project =>
            project.Id == id
            && (access.SeesAllProjects || (project.SupervisorId != null && project.SupervisorId == access.EmployeeId))));

    public Task<Project?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        GetByIdAsync(id, ProjectAccess.AllProjects, cancellationToken);

    public Task<PagedResult<Project>> ListAsync(int page, int size, string? search, ProjectAccess access, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public Task<Project?> GetForUpdateAsync(long id, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<bool> CodeExistsAsync(string code, long? ignoredProjectId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public void Add(Project project) => throw new NotImplementedException();

    public Task SaveChangesAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
}
