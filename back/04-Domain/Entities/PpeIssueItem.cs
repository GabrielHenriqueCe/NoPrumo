using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class PpeIssueItem
{
    public long Id { get; set; }

    public long PpeIssueId { get; set; }

    public long? StockItemId { get; set; }

    public string ItemNameSnapshot { get; set; } = null!;

    public string? CaSnapshot { get; set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual PpeIssue PpeIssue { get; set; } = null!;

    public virtual StockItem? StockItem { get; set; }
}
