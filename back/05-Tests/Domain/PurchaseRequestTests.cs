using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Tests.Domain;

public class PurchaseRequestTests
{
    private static readonly DateTime Now = new(2026, 10, 12, 14, 0, 0);

    [Fact]
    public void Approve_Pending_BecomesApprovedWithWhoAndWhen()
    {
        var request = new PurchaseRequest { Status = PurchaseRequestStatus.Pending, RejectionReason = "old" };

        Assert.True(request.Approve(decidedByUserId: 4, Now));
        Assert.Equal(PurchaseRequestStatus.Approved, request.Status);
        Assert.Equal(4, request.DecidedBy);
        Assert.Equal(Now, request.DecidedAt);
        Assert.Null(request.RejectionReason);
    }

    [Fact]
    public void Reject_Pending_KeepsTheReason()
    {
        var request = new PurchaseRequest { Status = PurchaseRequestStatus.Pending };

        Assert.True(request.Reject(4, Now, "Over budget"));
        Assert.Equal(PurchaseRequestStatus.Rejected, request.Status);
        Assert.Equal("Over budget", request.RejectionReason);
    }

    [Theory]
    [InlineData(PurchaseRequestStatus.Approved)]
    [InlineData(PurchaseRequestStatus.Rejected)]
    [InlineData(PurchaseRequestStatus.Purchased)]
    public void Approve_AlreadyDecided_ChangesNothing(PurchaseRequestStatus status)
    {
        var request = new PurchaseRequest { Status = status };

        Assert.False(request.Approve(4, Now));
        Assert.False(request.Reject(4, Now, "late"));
        Assert.Equal(status, request.Status);
    }

    [Fact]
    public void MarkPurchased_OnlyFromApproved()
    {
        var pending = new PurchaseRequest { Status = PurchaseRequestStatus.Pending };
        var approved = new PurchaseRequest { Status = PurchaseRequestStatus.Approved };

        Assert.False(pending.MarkPurchased(Now));
        Assert.True(approved.MarkPurchased(Now));
        Assert.Equal(PurchaseRequestStatus.Purchased, approved.Status);
    }

    [Fact]
    public void Fulfill_AddsUpPartialDeliveries()
    {
        var item = new PurchaseRequestItem { RequestedQuantity = 100m };

        item.Fulfill(60m);
        item.Fulfill(40m);

        Assert.Equal(100m, item.FulfilledQuantity);
    }
}
