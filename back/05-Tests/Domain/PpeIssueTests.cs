using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Tests.Domain;

public class PpeIssueTests
{
    [Fact]
    public void Sign_RecordsWhenAndTheHash()
    {
        var issue = new PpeIssue();
        var signedAt = new DateTime(2026, 10, 12, 9, 30, 0);

        issue.Sign(signedAt, "hash");

        Assert.True(issue.IsSigned());
        Assert.Equal(signedAt, issue.SignedAt);
        Assert.Equal("hash", issue.SignatureHash);
    }

    [Fact]
    public void IsSigned_NewIssue_IsFalse()
    {
        Assert.False(new PpeIssue().IsSigned());
    }

    [Fact]
    public void StockExits_OneExitPerItem_PointingBackToTheIssue()
    {
        var issue = new PpeIssue
        {
            Id = 42,
            EmployeeId = 7,
            ProjectId = 2,
            Items =
            [
                new PpeIssueItem { StockItemId = 1, Quantity = 1m, Unit = "un" },
                new PpeIssueItem { StockItemId = 5, Quantity = 2m, Unit = "par" },
            ],
        };

        var exits = issue.StockExits(new DateTime(2026, 10, 12), recordedBy: 3).ToList();

        Assert.Equal(2, exits.Count);
        Assert.All(exits, exit =>
        {
            Assert.Equal(StockMovementType.Exit, exit.Type);
            Assert.Equal(7, exit.EmployeeId);
            Assert.Equal(2, exit.ProjectId);
            Assert.Equal(42, exit.SourceId);
            Assert.Equal("ppe_issue", exit.SourceType);
            Assert.Equal(3, exit.RecordedBy);
        });
        Assert.Equal(2m, exits.Single(exit => exit.StockItemId == 5).Quantity);
    }
}
