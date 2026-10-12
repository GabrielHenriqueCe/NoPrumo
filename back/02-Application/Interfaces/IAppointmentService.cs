using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;

namespace NoPrumo.Application.Interfaces;

public interface IAppointmentService
{
    Task<AgendaResult> GetAgendaAsync(DateOnly? from, DateOnly? to, long? projectId, CancellationToken cancellationToken);

    Task<AgendaOptionsDto> GetOptionsAsync(CancellationToken cancellationToken);

    Task<AppointmentDto?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<SaveAppointmentResult> CreateAsync(SaveAppointmentRequest request, CancellationToken cancellationToken);

    Task<SaveAppointmentResult?> UpdateAsync(long id, SaveAppointmentRequest request, CancellationToken cancellationToken);

    Task<AppointmentDto?> CompleteAsync(long id, CancellationToken cancellationToken);

    Task<AppointmentDto?> ReopenAsync(long id, CancellationToken cancellationToken);
}
