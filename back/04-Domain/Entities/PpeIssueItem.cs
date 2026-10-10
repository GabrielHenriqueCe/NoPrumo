using System;
using System.Collections.Generic;

using NoPrumo.Domain.Enums;

namespace NoPrumo.Domain.Entities;

public class PpeIssueItem
{
    public long Id { get; set; }

    public long PpeIssueId { get; set; }

    public long? StockItemId { get; set; }

    public string ItemNameSnapshot { get; set; } = null!;

    public string? CaSnapshot { get; set; }

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public PpeItemStatus Status { get; set; }

    public DateOnly? IssueDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual PpeIssue PpeIssue { get; set; } = null!;

    public virtual StockItem? StockItem { get; set; }

    public bool IsIssued() => Status == PpeItemStatus.Issued;

    public bool ReturnsBeforeIssue(DateOnly returnDate) => returnDate < IssueDate;

    public void Return(DateOnly returnDate)
    {
        Status = PpeItemStatus.Returned;
        ReturnDate = returnDate;
    }

    public void MarkLost() => Status = PpeItemStatus.Lost;
}
