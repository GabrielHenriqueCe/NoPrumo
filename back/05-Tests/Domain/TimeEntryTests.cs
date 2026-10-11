using NoPrumo.Domain.Entities;
using Xunit;
using System;

namespace NoPrumo.Tests.Domain;

public class TimeEntryTests
{
    [Fact]
    public void CalculateHours_ShouldSubtractBreakTimeCorrectly()
    {
        // Arrange: Trabalhador das 08:00 às 17:00 com 1 hora de almoço.
        var entry = new TimeEntry
        {
            ClockIn = new TimeOnly(8, 0),
            ClockOut = new TimeOnly(17, 0),
            BreakStart = new TimeOnly(12, 0),
            BreakEnd = new TimeOnly(13, 0)
        };

        // Act
        var hours = entry.CalculateHours();

        // Assert: 9 horas de relógio - 1 hora de almoço = 8 horas de trabalho.
        Assert.Equal(8m, hours);
    }

    [Fact]
    public void CalculateCost_ShouldApplyCorrectMultipliers()
    {
        // Arrange: O documento dita a fórmula:
        // Custo = horas * valor-hora * (1 + adicional%) * (1 + encargos%)
        var entry = new TimeEntry
        {
            Hours = 8m,                    // 8 horas
            HourlyRateSnapshot = 10m,      // R$ 10,00 por hora
            AdditionalSnapshot = 30m,      // 30% periculosidade
            ChargesSnapshot = 71.80m       // 71.80% de encargo CLT
        };

        // Act
        var cost = entry.CalculateCost();

        // Assert:
        // Base = 8 * 10 = 80
        // Adicional (30%) = 80 * 1.3 = 104
        // Encargos (71.8%) = 104 * 1.718 = 178.672
        Assert.Equal(178.672m, cost);
    }
}