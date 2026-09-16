using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Subcontract
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long? SupplierId { get; set; }

    public long? StageId { get; set; }

    public string Description { get; set; } = null!;

    public string PriceType { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public decimal? UnitPrice { get; set; }

    public string? Unit { get; set; }

    public decimal? PlannedQuantity { get; set; }

    public decimal InssRetentionPct { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Stage? Stage { get; set; }

    public virtual Supplier? Supplier { get; set; }

    public virtual ICollection<SubcontractMeasurement> Measurements { get; set; } = new List<SubcontractMeasurement>();

    public virtual Project Project { get; set; } = null!;
}
