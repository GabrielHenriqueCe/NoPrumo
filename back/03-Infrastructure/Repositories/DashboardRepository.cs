using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class DashboardRepository(AppDbContext appDbContext) : IDashboardRepository
{
    public async Task<IReadOnlyList<Project>> ListActiveProjectsWithStagesAsync(CancellationToken cancellationToken) =>
        await appDbContext.Project
            .AsNoTracking()
            .Include(project => project.Stages)
            .Where(project => project.DeletedAt == null)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Employee>> ListActiveEmployeesWithTrainingsAsync(CancellationToken cancellationToken) =>
        await appDbContext.Employee
            .AsNoTracking()
            .Include(employee => employee.EmployeeTrainings)
            .Where(employee => employee.Active == true)
            .ToListAsync(cancellationToken);

    public Task<int> CountUnsignedPpeIssuesAsync(CancellationToken cancellationToken) =>
        appDbContext.PpeIssue.CountAsync(issue => issue.SignedAt == null, cancellationToken);
}
