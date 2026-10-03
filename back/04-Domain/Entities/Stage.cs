using NoPrumo.Domain.Enums;

namespace NoPrumo.Domain.Entities;

public class Stage
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public int SortOrder { get; set; }

    public long? TeamId { get; set; }

    public long? SupervisorId { get; set; }

    public DateOnly? PlannedDate { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? CompletionDate { get; set; }

    public decimal Percentage { get; set; }

    public StageStatus Status { get; set; }

    public string? Notes { get; set; }

    public long? MarkedBy { get; set; }

    public DateTime? MarkedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Subcontract> Subcontracts { get; set; } = new List<Subcontract>();

    public virtual Team? Team { get; set; }

    public virtual User? MarkedByUser { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual Employee? Supervisor { get; set; }

    public bool IsLate(DateOnly today) =>
    PlannedDate < today
    && CompletionDate is null
    && Status is not StageStatus.Completed;

    public bool IsCompletedWithPartialProgress() =>
    Status is StageStatus.Completed && Percentage < 100;
}
