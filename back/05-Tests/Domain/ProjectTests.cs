using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Tests.Domain;

public class ProjectTests
{
    private static readonly DateOnly Today = new(2026, 10, 12);

    [Fact]
    public void IsLate_ForecastPassedAndNotFinished_IsLate()
    {
        var project = new Project { ForecastDate = Today.AddDays(-1), Status = ProjectStatus.InProgress };

        Assert.True(project.IsLate(Today));
    }

    [Fact]
    public void IsLate_ForecastIsToday_IsNotLate()
    {
        var project = new Project { ForecastDate = Today, Status = ProjectStatus.InProgress };

        Assert.False(project.IsLate(Today));
    }

    [Theory]
    [InlineData(ProjectStatus.Completed)]
    [InlineData(ProjectStatus.Cancelled)]
    public void IsLate_FinishedProject_IsNeverLate(ProjectStatus status)
    {
        var project = new Project { ForecastDate = Today.AddDays(-30), Status = status };

        Assert.False(project.IsLate(Today));
    }

    [Fact]
    public void IsLate_WithCompletionDate_IsNotLate()
    {
        var project = new Project
        {
            ForecastDate = Today.AddDays(-30),
            CompletionDate = Today.AddDays(-2),
            Status = ProjectStatus.InProgress,
        };

        Assert.False(project.IsLate(Today));
    }

    [Fact]
    public void IsForecastBeforeStart_ForecastBeforeStart_IsTrue()
    {
        var project = new Project { StartDate = Today, ForecastDate = Today.AddDays(-1) };

        Assert.True(project.IsForecastBeforeStart());
    }

    [Fact]
    public void IsForecastBeforeStart_WithoutDates_IsFalse()
    {
        Assert.False(new Project().IsForecastBeforeStart());
    }

    [Theory]
    [InlineData(ProjectStatus.Planning, true)]
    [InlineData(ProjectStatus.InProgress, true)]
    [InlineData(ProjectStatus.OnHold, true)]
    [InlineData(ProjectStatus.Completed, false)]
    [InlineData(ProjectStatus.Cancelled, false)]
    public void IsUnderway_DependsOnStatus(ProjectStatus status, bool expected)
    {
        Assert.Equal(expected, new Project { Status = status }.IsUnderway());
    }

    [Fact]
    public void IsUnderway_DeactivatedProject_IsNotUnderway()
    {
        var project = new Project { Status = ProjectStatus.InProgress };
        project.Deactivate(new DateTime(2026, 10, 1));

        Assert.False(project.IsUnderway());
    }

    [Fact]
    public void Deactivate_Twice_KeepsTheFirstDate()
    {
        var project = new Project();
        var first = new DateTime(2026, 10, 1);

        project.Deactivate(first);
        project.Deactivate(first.AddDays(5));

        Assert.Equal(first, project.DeletedAt);
    }

    [Fact]
    public void RevisedContractAmount_AddsEveryAmendment()
    {
        var project = new Project
        {
            ContractAmount = 1_000_000m,
            Amendments = [new ProjectAmendment { Amount = 85_000m }, new ProjectAmendment { Amount = -15_000m }],
        };

        Assert.Equal(1_070_000m, project.RevisedContractAmount());
    }

    [Fact]
    public void ProgressPercentage_IsTheAverageOfTheStages()
    {
        var project = new Project
        {
            Stages = [new Stage { Percentage = 100m }, new Stage { Percentage = 50m }, new Stage { Percentage = 0m }],
        };

        Assert.Equal(50m, project.ProgressPercentage());
    }

    [Fact]
    public void ProgressPercentage_WithoutStages_IsZero()
    {
        Assert.Equal(0m, new Project().ProgressPercentage());
    }

    [Fact]
    public void SpentPercentage_IsCostOverRevisedContract()
    {
        var project = new Project { ContractAmount = 2_000_000m };

        Assert.Equal(7.5m, project.SpentPercentage(150_000m));
    }

    [Fact]
    public void SpentPercentage_WithoutContract_IsZeroInsteadOfDividingByZero()
    {
        Assert.Equal(0m, new Project { ContractAmount = 0m }.SpentPercentage(10_000m));
    }

    [Fact]
    public void IsCostAheadOfProgress_SpentMoreThanBuilt_IsTrue()
    {
        var project = new Project
        {
            ContractAmount = 1_000_000m,
            Stages = [new Stage { Percentage = 20m }],
        };

        // 25% do contrato gasto com 20% da obra pronta.
        Assert.True(project.IsCostAheadOfProgress(250_000m));
    }

    [Fact]
    public void IsCostAheadOfProgress_BuiltMoreThanSpent_IsFalse()
    {
        var project = new Project
        {
            ContractAmount = 1_000_000m,
            Stages = [new Stage { Percentage = 20m }],
        };

        Assert.False(project.IsCostAheadOfProgress(100_000m));
    }

    [Fact]
    public void Margin_IsRevisedContractMinusCost()
    {
        var project = new Project
        {
            ContractAmount = 500_000m,
            Amendments = [new ProjectAmendment { Amount = 50_000m }],
        };

        Assert.Equal(150_000m, project.Margin(400_000m));
    }
}
