using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

public sealed record StageDto
{
    public required long Id { get; init; }
    public required long ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public required string Name { get; init; }
    public required int SortOrder { get; init; }
    public required long? TeamId { get; init; }
    public required string? TeamName { get; init; }
    public required long? SupervisorId { get; init; }
    public required string? SupervisorName { get; init; }
    public required DateOnly? PlannedDate { get; init; }
    public required DateOnly? StartDate { get; init; }
    public required DateOnly? CompletionDate { get; init; }
    public required decimal Percentage { get; init; }
    public required StageStatus Status { get; init; }
    public required string? Notes { get; init; }
    public required bool Late { get; init; }
}
