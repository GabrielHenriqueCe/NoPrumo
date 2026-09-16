using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class ProjectAmendment
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public string Type { get; set; } = null!;

    public string? Number { get; set; }

    public DateOnly Date { get; set; }

    public decimal Amount { get; set; }

    public int ExtraDays { get; set; }

    public string? Reason { get; set; }

    public long? ApprovedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual Project Project { get; set; } = null!;
}
