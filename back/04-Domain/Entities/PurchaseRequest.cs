using System;
using System.Collections.Generic;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Domain.Entities;

public class PurchaseRequest
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long? RequestedBy { get; set; }

    public DateOnly Date { get; set; }

    public DateOnly? NeededByDate { get; set; }

    public PurchaseRequestStatus Status { get; set; }

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

    public bool Approve(long decidedByUserId, DateTime decidedAtUtc)
    {
        if (Status != PurchaseRequestStatus.Pending)
        {
            return false;
        }

        Status = PurchaseRequestStatus.Approved;
        DecidedBy = decidedByUserId;
        DecidedAt = decidedAtUtc;
        RejectionReason = null;
        UpdatedAt = decidedAtUtc;
        return true;
    }

    public bool Reject(long decidedByUserId, DateTime decidedAtUtc, string reason)
    {
        if (Status != PurchaseRequestStatus.Pending)
        {
            return false;
        }

        Status = PurchaseRequestStatus.Rejected;
        DecidedBy = decidedByUserId;
        DecidedAt = decidedAtUtc;
        RejectionReason = reason;
        UpdatedAt = decidedAtUtc;
        return true;
    }

    public bool MarkPurchased(DateTime purchasedAtUtc)
    {
        if (Status != PurchaseRequestStatus.Approved)
        {
            return false;
        }

        Status = PurchaseRequestStatus.Purchased;
        UpdatedAt = purchasedAtUtc;
        return true;
    }
}
