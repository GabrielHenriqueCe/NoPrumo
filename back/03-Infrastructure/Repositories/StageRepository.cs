using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class StageRepository(AppDbContext appDbContext) : IStageRepository
{
    public async Task<PagedResult<Stage>> ListAsync(int page, int size, string? search, long? projectId, CancellationToken cancellationToken)
    {
        var query = appDbContext.Stage.AsNoTracking();

        if (projectId is not null)
        {
            query = query.Where(stage => stage.ProjectId == projectId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(stage =>
                stage.Name.Contains(term) ||
                stage.Project.Name.Contains(term));
        }

        var total = await query.CountAsync(cancellationToken);

        var stages = await query
            .Include(stage => stage.Project)
            .Include(stage => stage.Team)
            .Include(stage => stage.Supervisor)
            .OrderBy(stage => stage.Project.Name)
            .ThenBy(stage => stage.SortOrder)
            .ThenBy(stage => stage.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));

        return new PagedResult<Stage>(stages, page, size, total, totalPages);
    }

    public Task<Stage?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        appDbContext.Stage
            .AsNoTracking()
            .Include(stage => stage.Project)
            .Include(stage => stage.Team)
            .Include(stage => stage.Supervisor)
            .FirstOrDefaultAsync(stage => stage.Id == id, cancellationToken);

    public Task<Stage?> GetForUpdateAsync(long id, CancellationToken cancellationToken) =>
    appDbContext.Stage.FirstOrDefaultAsync(stage => stage.Id == id, cancellationToken);

    public Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken) =>
        appDbContext.Project.AnyAsync(project =>
            project.Id == projectId &&
            project.DeletedAt == null,
            cancellationToken);

    public void Add(Stage stage) => appDbContext.Stage.Add(stage);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        appDbContext.SaveChangesAsync(cancellationToken);

}
