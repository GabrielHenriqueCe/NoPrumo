using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class PurchaseRequest
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long? RequestedBy { get; set; }

    public DateOnly Date { get; set; }

    public DateOnly? NeededByDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public long? DecidedBy { get; set; }

    public DateTime? DecidedAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User? DecidedByUser { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<PurchaseRequestItem> Items { get; set; } = new List<PurchaseRequestItem>();

    public virtual User? RequestedByUser { get; set; }
}
