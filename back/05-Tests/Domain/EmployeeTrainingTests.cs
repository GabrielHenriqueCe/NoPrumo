using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Tests.Domain;

public class EmployeeTrainingTests
{
    private static readonly DateOnly Today = new(2026, 10, 12);

    [Fact]
    public void StatusOn_WithoutExpiry_IsNoExpiry()
    {
        Assert.Equal(TrainingStatus.NoExpiry, new EmployeeTraining().StatusOn(Today));
    }

    [Fact]
    public void StatusOn_ExpiredYesterday_IsExpired()
    {
        var training = new EmployeeTraining { ExpiryDate = Today.AddDays(-1) };

        Assert.Equal(TrainingStatus.Expired, training.StatusOn(Today));
    }

    [Fact]
    public void StatusOn_ExpiresToday_IsExpiringNotExpired()
    {
        var training = new EmployeeTraining { ExpiryDate = Today };

        Assert.Equal(TrainingStatus.Expiring, training.StatusOn(Today));
    }

    [Fact]
    public void StatusOn_ExpiresInThirtyDays_IsExpiring()
    {
        var training = new EmployeeTraining { ExpiryDate = Today.AddDays(30) };

        Assert.Equal(TrainingStatus.Expiring, training.StatusOn(Today));
    }

    [Fact]
    public void StatusOn_ExpiresInThirtyOneDays_IsValid()
    {
        var training = new EmployeeTraining { ExpiryDate = Today.AddDays(31) };

        Assert.Equal(TrainingStatus.Valid, training.StatusOn(Today));
    }

    [Theory]
    [InlineData(-5, true)]
    [InlineData(10, true)]
    [InlineData(90, false)]
    public void NeedsAttentionOn_OnlyExpiredOrExpiring(int daysToExpiry, bool expected)
    {
        var training = new EmployeeTraining { ExpiryDate = Today.AddDays(daysToExpiry) };

        Assert.Equal(expected, training.NeedsAttentionOn(Today));
    }
}
