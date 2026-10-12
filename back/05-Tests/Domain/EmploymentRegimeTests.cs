using NoPrumo.Domain.Entities;
using Xunit;

namespace NoPrumo.Tests.Domain;

public class EmploymentRegimeTests
{
    [Fact]
    public void HourlyRate_MonthlySalary_DividesByMonthlyHours()
    {
        var regime = new EmploymentRegime { Unit = "month", MonthlyHours = 220m };

        Assert.Equal(30m, regime.HourlyRate(6600m));
    }

    [Fact]
    public void HourlyRate_MonthlyWithoutHours_UsesCltJourney()
    {
        var regime = new EmploymentRegime { Unit = "month", MonthlyHours = null };

        Assert.Equal(30m, regime.HourlyRate(6600m));
    }

    [Fact]
    public void HourlyRate_DailyRate_DividesByEightHours()
    {
        var regime = new EmploymentRegime { Unit = "day" };

        Assert.Equal(20m, regime.HourlyRate(160m));
    }

    [Fact]
    public void HourlyRate_HourlyRate_StaysTheSame()
    {
        var regime = new EmploymentRegime { Unit = "hour" };

        Assert.Equal(24.50m, regime.HourlyRate(24.50m));
    }
}
