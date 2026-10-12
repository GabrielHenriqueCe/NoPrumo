using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class AppointmentRepository(AppDbContext appDbContext) : IAppointmentRepository
{
    public async Task<IReadOnlyList<Appointment>> ListAsync(DateOnly from, DateOnly to, long? projectId, CancellationToken cancellationToken)
    {
        var query = appDbContext.Appointment
            .AsNoTracking()
            .Where(appointment => appointment.Date >= from && appointment.Date <= to);

        if (projectId is not null)
        {
            query = query.Where(appointment => appointment.ProjectId == projectId);
        }

        return await query
            .Include(appointment => appointment.Project)
            .Include(appointment => appointment.AssignedTo)
            .OrderBy(appointment => appointment.Date)
            .ThenBy(appointment => appointment.Time)
            .ThenBy(appointment => appointment.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EmployeeTraining>> ListExpiringTrainingsAsync(DateOnly from, DateOnly to, long? projectId, CancellationToken cancellationToken)
    {
        // O mesmo critério de Employee.LatestTrainings, escrito em SQL: um
        // certificado que já foi renovado não vence mais.
        var query = appDbContext.EmployeeTraining
            .AsNoTracking()
            .Where(training =>
                training.ExpiryDate >= from &&
                training.ExpiryDate <= to &&
                training.Employee.Active == true &&
                !appDbContext.EmployeeTraining.Any(newer =>
                    newer.EmployeeId == training.EmployeeId &&
                    newer.TrainingTypeId == training.TrainingTypeId &&
                    (newer.IssueDate > training.IssueDate ||
                     (newer.IssueDate == training.IssueDate && newer.Id > training.Id))));

        // Treinamento não tem obra: na obra, conta quem está hoje numa equipe alocada nela.
        if (projectId is not null)
        {
            query = query.Where(training => appDbContext.TeamProject.Any(allocation =>
                allocation.ProjectId == projectId &&
                allocation.EndDate == null &&
                allocation.Team.EmployeeTeams.Any(membership =>
                    membership.EmployeeId == training.EmployeeId &&
                    membership.EndDate == null)));
        }

        return await query
            .Include(training => training.Employee)
            .Include(training => training.TrainingType)
            .OrderBy(training => training.ExpiryDate)
            .ThenBy(training => training.Employee.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Appointment?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        appDbContext.Appointment
            .AsNoTracking()
            .Include(appointment => appointment.Project)
            .Include(appointment => appointment.AssignedTo)
            .FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken);

    public Task<Appointment?> GetForUpdateAsync(long id, CancellationToken cancellationToken) =>
        appDbContext.Appointment.FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken);

    public Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken) =>
        appDbContext.Project.AnyAsync(project => project.Id == projectId && project.DeletedAt == null, cancellationToken);

    public Task<bool> ActiveUserExistsAsync(long userId, CancellationToken cancellationToken) =>
        appDbContext.User.AnyAsync(user => user.Id == userId && user.Active == true, cancellationToken);

    public async Task<IReadOnlyList<Project>> ListActiveProjectsAsync(CancellationToken cancellationToken) =>
        await appDbContext.Project
            .AsNoTracking()
            .Where(project => project.DeletedAt == null)
            .OrderBy(project => project.Name)
            .ThenBy(project => project.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<User>> ListActiveUsersAsync(CancellationToken cancellationToken) =>
        await appDbContext.User
            .AsNoTracking()
            .Where(user => user.Active == true)
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Id)
            .ToListAsync(cancellationToken);

    public void Add(Appointment appointment) => appDbContext.Appointment.Add(appointment);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        appDbContext.SaveChangesAsync(cancellationToken);
}
