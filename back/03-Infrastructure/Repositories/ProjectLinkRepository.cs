using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class ProjectLinkRepository(AppDbContext appDbContext) : IProjectLinkRepository
{
    public Task<ProjectLink?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        appDbContext.ProjectLink
            .Include(link => link.Project)
                .ThenInclude(project => project.Stages)
            .FirstOrDefaultAsync(link => link.TokenHash == tokenHash, cancellationToken);

    public Task<ProjectLink?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        appDbContext.ProjectLink
            .FirstOrDefaultAsync(link => link.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProjectLink>> ListByProjectIdAsync(long projectId, CancellationToken cancellationToken)
    {
        var links = await appDbContext.ProjectLink
            .AsNoTracking()
            .Where(link => link.ProjectId == projectId)
            .OrderByDescending(link => link.CreatedAt)
            .ToListAsync(cancellationToken);

        return links;
    }

    public void Add(ProjectLink projectLink) => appDbContext.ProjectLink.Add(projectLink);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        appDbContext.SaveChangesAsync(cancellationToken);
}
