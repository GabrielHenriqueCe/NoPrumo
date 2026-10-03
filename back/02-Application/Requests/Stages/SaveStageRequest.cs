using System.ComponentModel.DataAnnotations;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.Requests.Stages;

public sealed record SaveStageRequest
{
    [Range(1, long.MaxValue, ErrorMessage = "Pick a project.")]
    public long ProjectId { get; init; }

    [Required(ErrorMessage = "Enter the stage name.")]
    [MaxLength(180)]
    public string Name { get; init; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Use zero or more.")]
    public int SortOrder { get; init; }

    public long? TeamId { get; init; }

    public long? SupervisorId { get; init; }

    public DateOnly? PlannedDate { get; init; }

    [Range(typeof(decimal), "0", "100", ParseLimitsInInvariantCulture = true, ErrorMessage = "Enter a percentage from 0 to 100.")]
    public decimal Percentage { get; init; }

    public StageStatus Status { get; init; } = StageStatus.Planned;
}