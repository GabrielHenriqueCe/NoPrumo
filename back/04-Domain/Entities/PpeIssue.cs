using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class PpeIssue
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long? ProjectId { get; set; }

    public DateOnly Date { get; set; }

    public string? Notes { get; set; }

    public long? IssuedById { get; set; }

    public DateTime? SignedAt { get; set; }

    public string? SignatureUrl { get; set; }

    public string? SignatureHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<PpeIssueItem> Items { get; set; } = new List<PpeIssueItem>();

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project? Project { get; set; }

    public virtual User? IssuedBy { get; set; }
}
