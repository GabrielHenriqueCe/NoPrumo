using NoPrumo.Application.DTOs;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Services;

public sealed class AppointmentService(IAppointmentRepository appointmentRepository, TimeProvider timeProvider) : IAppointmentService
{
    private const int MaxRangeDays = 92;
    private const int MaxTitleLength = 220;
    private const int MaxDescriptionLength = 2000;

    public async Task<AgendaResult> GetAgendaAsync(DateOnly? from, DateOnly? to, long? projectId, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string>();

        if (from is null)
        {
            errors["from"] = "Pick the first day.";
        }

        if (to is null)
        {
            errors["to"] = "Pick the last day.";
        }
        else if (from is not null && to < from)
        {
            errors["to"] = "The last day cannot be before the first.";
        }
        else if (from is not null && to.Value.DayNumber - from.Value.DayNumber > MaxRangeDays)
        {
            errors["to"] = $"Show at most {MaxRangeDays} days at a time.";
        }

        if (errors.Count > 0)
        {
            return new AgendaResult(null, errors);
        }

        var appointments = await appointmentRepository.ListAsync(from!.Value, to!.Value, projectId, cancellationToken);
        var trainings = await appointmentRepository.ListExpiringTrainingsAsync(from.Value, to.Value, projectId, cancellationToken);
        var today = timeProvider.Today();

        var agenda = new AgendaDto
        {
            From = from.Value,
            To = to.Value,
            Appointments = appointments.Select(ToDto).ToList(),
            TrainingExpiries = trainings.Select(training => ToExpiryDto(training, today)).ToList(),
        };

        return new AgendaResult(agenda, errors);
    }

    public async Task<AgendaOptionsDto> GetOptionsAsync(CancellationToken cancellationToken)
    {
        var projects = await appointmentRepository.ListActiveProjectsAsync(cancellationToken);
        var users = await appointmentRepository.ListActiveUsersAsync(cancellationToken);

        return new AgendaOptionsDto
        {
            Projects = projects.Select(project => new OptionDto { Id = project.Id, Name = project.Name }).ToList(),
            Assignees = users.Select(user => new OptionDto { Id = user.Id, Name = user.Name }).ToList(),
        };
    }

    public async Task<AppointmentDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(id, cancellationToken);
        return appointment is null ? null : ToDto(appointment);
    }

    public async Task<SaveAppointmentResult> CreateAsync(SaveAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = new Appointment();
        var errors = await ValidateAsync(request, appointment, cancellationToken);
        if (errors.Count > 0)
        {
            return new SaveAppointmentResult(null, errors);
        }

        Apply(request, appointment);
        appointmentRepository.Add(appointment);
        await appointmentRepository.SaveChangesAsync(cancellationToken);

        return new SaveAppointmentResult(await GetByIdAsync(appointment.Id, cancellationToken), errors);
    }

    public async Task<SaveAppointmentResult?> UpdateAsync(long id, SaveAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetForUpdateAsync(id, cancellationToken);
        if (appointment is null)
        {
            return null;
        }

        var errors = await ValidateAsync(request, appointment, cancellationToken);
        if (errors.Count > 0)
        {
            return new SaveAppointmentResult(null, errors);
        }

        Apply(request, appointment);
        await appointmentRepository.SaveChangesAsync(cancellationToken);

        return new SaveAppointmentResult(await GetByIdAsync(id, cancellationToken), errors);
    }

    public async Task<AppointmentDto?> CompleteAsync(long id, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetForUpdateAsync(id, cancellationToken);
        if (appointment is null)
        {
            return null;
        }

        appointment.Complete(timeProvider.Now());
        await appointmentRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<AppointmentDto?> ReopenAsync(long id, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetForUpdateAsync(id, cancellationToken);
        if (appointment is null)
        {
            return null;
        }

        appointment.Reopen();
        await appointmentRepository.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken);
    }

    private async Task<Dictionary<string, string>> ValidateAsync(SaveAppointmentRequest request, Appointment current, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string>();
        var title = request.Title?.Trim() ?? string.Empty;

        if (request.Date is null)
        {
            errors["date"] = "Date is required.";
        }

        if (title.Length == 0)
        {
            errors["title"] = "Title is required.";
        }
        else if (title.Length > MaxTitleLength)
        {
            errors["title"] = $"Use at most {MaxTitleLength} characters.";
        }

        if (request.Description?.Trim().Length > MaxDescriptionLength)
        {
            errors["description"] = $"Use at most {MaxDescriptionLength} characters.";
        }

        // Obra ou responsável desativados depois continuam no compromisso antigo.
        if (request.ProjectId is { } projectId
            && projectId != current.ProjectId
            && !await appointmentRepository.ActiveProjectExistsAsync(projectId, cancellationToken))
        {
            errors["projectId"] = "Pick an active project.";
        }

        if (request.AssignedToId is { } userId
            && userId != current.AssignedToId
            && !await appointmentRepository.ActiveUserExistsAsync(userId, cancellationToken))
        {
            errors["assignedToId"] = "Pick an active user.";
        }

        return errors;
    }

    private static void Apply(SaveAppointmentRequest request, Appointment appointment)
    {
        appointment.Date = request.Date!.Value;
        appointment.Time = request.Time;
        appointment.Title = request.Title!.Trim();
        appointment.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        appointment.ProjectId = request.ProjectId;
        appointment.AssignedToId = request.AssignedToId;
        appointment.Type = request.Type;
    }

    private static AppointmentDto ToDto(Appointment appointment) => new()
    {
        Id = appointment.Id,
        Date = appointment.Date,
        Time = appointment.Time,
        Title = appointment.Title,
        Description = appointment.Description,
        ProjectId = appointment.ProjectId,
        ProjectName = appointment.Project?.Name,
        AssignedToId = appointment.AssignedToId,
        AssignedToName = appointment.AssignedTo?.Name,
        Type = appointment.Type,
        Completed = appointment.Completed,
        CompletedAt = appointment.CompletedAt,
    };

    private static TrainingExpiryDto ToExpiryDto(EmployeeTraining training, DateOnly today) => new()
    {
        TrainingId = training.Id,
        EmployeeId = training.EmployeeId,
        EmployeeName = training.Employee.Name,
        TrainingTypeCode = training.TrainingType.Code,
        TrainingTypeName = training.TrainingType.Name,
        ExpiryDate = training.ExpiryDate!.Value,
        Status = training.StatusOn(today),
    };
}
