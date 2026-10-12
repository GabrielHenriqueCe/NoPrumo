namespace NoPrumo.Application.DTOs;

public sealed record AgendaDto
{
    public required DateOnly From { get; init; }
    public required DateOnly To { get; init; }
    public required IReadOnlyList<AppointmentDto> Appointments { get; init; }
    public required IReadOnlyList<TrainingExpiryDto> TrainingExpiries { get; init; }
}
