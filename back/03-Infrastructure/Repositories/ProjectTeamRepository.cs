using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class ProjectTeamRepository(AppDbContext appDbContext) : IProjectTeamRepository
{
    public Task<bool> ProjectExistsAsync(long projectId, CancellationToken cancellationToken) =>
        appDbContext.Project.AnyAsync(project => project.Id == projectId, cancellationToken);

    public Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken) =>
        appDbContext.Project.AnyAsync(project => project.Id == projectId && project.DeletedAt == null, cancellationToken);

    public Task<Team?> GetTeamAsync(long teamId, CancellationToken cancellationToken) =>
        appDbContext.Team.AsNoTracking().FirstOrDefaultAsync(team => team.Id == teamId, cancellationToken);

    public async Task<IReadOnlyList<Team>> ListActiveTeamsAsync(CancellationToken cancellationToken) =>
        await appDbContext.Team
            .AsNoTracking()
            .Include(team => team.Department)
            .Where(team => team.DeletedAt == null)
            .OrderBy(team => team.Name)
            .ThenBy(team => team.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TeamProject>> ListByProjectAsync(long projectId, CancellationToken cancellationToken) =>
        await WithDetails()
            .Where(allocation => allocation.ProjectId == projectId)
            .OrderBy(allocation => allocation.EndDate != null)
            .ThenByDescending(allocation => allocation.StartDate)
            .ThenBy(allocation => allocation.Team.Name)
            .ToListAsync(cancellationToken);

    public Task<TeamProject?> GetAsync(long projectId, long teamId, DateOnly startDate, CancellationToken cancellationToken) =>
        WithDetails().FirstOrDefaultAsync(allocation =>
            allocation.ProjectId == projectId &&
            allocation.TeamId == teamId &&
            allocation.StartDate == startDate,
            cancellationToken);

    public async Task<IReadOnlyList<TeamProject>> ListPeriodsAsync(long projectId, long teamId, CancellationToken cancellationToken) =>
        await appDbContext.TeamProject
            .AsNoTracking()
            .Where(allocation => allocation.ProjectId == projectId && allocation.TeamId == teamId)
            .OrderByDescending(allocation => allocation.StartDate)
            .ToListAsync(cancellationToken);

    public Task<TeamProject?> GetOpenForUpdateAsync(long projectId, long teamId, CancellationToken cancellationToken) =>
        appDbContext.TeamProject
            .Where(allocation =>
                allocation.ProjectId == projectId &&
                allocation.TeamId == teamId &&
                allocation.EndDate == null)
            .OrderByDescending(allocation => allocation.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

    public void Add(TeamProject allocation) => appDbContext.TeamProject.Add(allocation);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        appDbContext.SaveChangesAsync(cancellationToken);

    // Quem está na equipe hoje, com os treinamentos, para o alerta de NR vencendo.
    private IQueryable<TeamProject> WithDetails() =>
        appDbContext.TeamProject
            .AsNoTracking()
            .Include(allocation => allocation.Team)
                .ThenInclude(team => team.Department)
            .Include(allocation => allocation.Team)
                .ThenInclude(team => team.EmployeeTeams.Where(membership => membership.EndDate == null))
                    .ThenInclude(membership => membership.Employee)
                        .ThenInclude(employee => employee.EmployeeTrainings)
                            .ThenInclude(training => training.TrainingType)
            .AsSplitQuery();
}
