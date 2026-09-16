using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class SubcontractMeasurement
{
    public long Id { get; set; }

    public long SubcontractId { get; set; }

    public int Number { get; set; }

    public DateOnly Date { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? Percentage { get; set; }

    public decimal Amount { get; set; }

    public long? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual Subcontract Subcontract { get; set; } = null!;
}
