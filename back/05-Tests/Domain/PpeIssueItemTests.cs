using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Tests.Domain;

public class PpeIssueItemTests
{
    private static readonly DateOnly IssueDate = new(2026, 10, 1);

    [Fact]
    public void Return_ClosesTheItemWithTheDate()
    {
        var item = new PpeIssueItem { Status = PpeItemStatus.Issued, IssueDate = IssueDate };

        item.Return(IssueDate.AddDays(10));

        Assert.Equal(PpeItemStatus.Returned, item.Status);
        Assert.Equal(IssueDate.AddDays(10), item.ReturnDate);
        Assert.False(item.IsIssued());
    }

    [Fact]
    public void MarkLost_ClosesTheItemAsLost()
    {
        var item = new PpeIssueItem { Status = PpeItemStatus.Issued };

        item.MarkLost();

        Assert.Equal(PpeItemStatus.Lost, item.Status);
    }

    [Fact]
    public void ReturnsBeforeIssue_DateBeforeIssue_IsTrue()
    {
        var item = new PpeIssueItem { IssueDate = IssueDate };

        Assert.True(item.ReturnsBeforeIssue(IssueDate.AddDays(-1)));
        Assert.False(item.ReturnsBeforeIssue(IssueDate));
    }
}
