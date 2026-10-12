using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

public sealed record AppointmentDto
{
    public required long Id { get; init; }
    public required DateOnly Date { get; init; }
    public required TimeOnly? Time { get; init; }
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required long? ProjectId { get; init; }
    public required string? ProjectName { get; init; }
    public required long? AssignedToId { get; init; }
    public required string? AssignedToName { get; init; }
    public required AppointmentType Type { get; init; }
    public required bool Completed { get; init; }
    public required DateTime? CompletedAt { get; init; }
}
