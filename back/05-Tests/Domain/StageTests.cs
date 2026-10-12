using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Tests.Domain;

public class StageTests
{
    private static readonly DateOnly Today = new(2026, 10, 12);

    [Fact]
    public void IsLate_PlannedDatePassedAndNotCompleted_IsLate()
    {
        var stage = new Stage { PlannedDate = Today.AddDays(-1), Status = StageStatus.InProgress };

        Assert.True(stage.IsLate(Today));
    }

    [Fact]
    public void IsLate_Completed_IsNotLate()
    {
        var stage = new Stage { PlannedDate = Today.AddDays(-10), Status = StageStatus.Completed };

        Assert.False(stage.IsLate(Today));
    }

    [Fact]
    public void IsLate_WithoutPlannedDate_IsNotLate()
    {
        Assert.False(new Stage { Status = StageStatus.Planned }.IsLate(Today));
    }

    [Fact]
    public void IsCompletedWithPartialProgress_CompletedBelowHundred_IsTrue()
    {
        var stage = new Stage { Status = StageStatus.Completed, Percentage = 90m };

        Assert.True(stage.IsCompletedWithPartialProgress());
    }

    [Fact]
    public void IsCompletedWithPartialProgress_CompletedAtHundred_IsFalse()
    {
        var stage = new Stage { Status = StageStatus.Completed, Percentage = 100m };

        Assert.False(stage.IsCompletedWithPartialProgress());
    }
}
