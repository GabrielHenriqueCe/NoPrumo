using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class PayrollChargeRate
{
    public long Id { get; set; }

    public long EmploymentRegimeId { get; set; }

    public decimal Percentage { get; set; }

    public DateOnly EffectiveStart { get; set; }

    public DateOnly? EffectiveEnd { get; set; }

    public string Source { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual EmploymentRegime EmploymentRegime { get; set; } = null!;
}
