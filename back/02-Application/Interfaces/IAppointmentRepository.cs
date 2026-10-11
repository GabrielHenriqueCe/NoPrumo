using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<IReadOnlyList<Appointment>> ListAsync(DateOnly from, DateOnly to, long? projectId, CancellationToken cancellationToken);

    Task<IReadOnlyList<EmployeeTraining>> ListExpiringTrainingsAsync(DateOnly from, DateOnly to, long? projectId, CancellationToken cancellationToken);

    Task<Appointment?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<Appointment?> GetForUpdateAsync(long id, CancellationToken cancellationToken);

    Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken);

    Task<bool> ActiveUserExistsAsync(long userId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Project>> ListActiveProjectsAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<User>> ListActiveUsersAsync(CancellationToken cancellationToken);

    void Add(Appointment appointment);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
