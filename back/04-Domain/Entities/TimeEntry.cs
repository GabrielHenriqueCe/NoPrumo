using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class TimeEntry
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long ProjectId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? ClockIn { get; set; }

    public TimeOnly? BreakStart { get; set; }

    public TimeOnly? BreakEnd { get; set; }

    public TimeOnly? ClockOut { get; set; }

    public decimal? Hours { get; set; }

    public decimal HourlyRateSnapshot { get; set; }

    public decimal AdditionalSnapshot { get; set; }

    public decimal ChargesSnapshot { get; set; }

    public decimal Cost { get; set; }

    public string Source { get; set; } = null!;

    public string? ExternalId { get; set; }

    public long? RecordedBy { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual User? RecordedByUser { get; set; }
}
