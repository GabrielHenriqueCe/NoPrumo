using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.Requests;

public sealed record SaveAppointmentRequest
{
    public DateOnly? Date { get; init; }
    public TimeOnly? Time { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public long? ProjectId { get; init; }
    public long? AssignedToId { get; init; }
    public AppointmentType Type { get; init; } = AppointmentType.Project;
}
