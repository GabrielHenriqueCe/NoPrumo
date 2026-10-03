using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class ProjectRepository(AppDbContext appDbContext) : IProjectRepository
{
    public async Task<PagedResult<Project>> ListAsync(int page, int size, string? search, CancellationToken cancellationToken)
    {
        var query = appDbContext.Project.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(project =>
                project.Code.Contains(term) ||
                project.Name.Contains(term) ||
                (project.City != null && project.City.Contains(term)));
        }

        var total = await query.CountAsync(cancellationToken);

        var projects = await query
            .Include(project => project.Client)
            .Include(project => project.Supervisor)
            .OrderBy(project => project.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));

        return new PagedResult<Project>(projects, page, size, total, totalPages);
    }

    public Task<Project?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        appDbContext.Project
            .AsNoTracking()
            .Include(project => project.Client)
            .Include(project => project.Supervisor)
            .FirstOrDefaultAsync(project => project.Id == id, cancellationToken);
}