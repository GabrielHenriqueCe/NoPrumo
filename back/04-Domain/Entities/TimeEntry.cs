using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public class TimeEntry
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long ProjectId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? ClockIn { get; set; }

    public TimeOnly? BreakStart { get; set; }

    public TimeOnly? BreakEnd { get; set; }

    public TimeOnly? ClockOut { get; set; }

    public decimal? Hours { get; set; }

    public decimal HourlyRateSnapshot { get; set; }

    public decimal AdditionalSnapshot { get; set; }

    public decimal ChargesSnapshot { get; set; }

    public decimal Cost { get; set; }

    public string Source { get; set; } = null!;

    public string? ExternalId { get; set; }

    public long? RecordedBy { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual User? RecordedByUser { get; set; }

    // --- Métodos de Lógica de Negócio (Domínio) ---

    public decimal CalculateHours()
    {
        if (!ClockIn.HasValue || !ClockOut.HasValue) return 0;

        // Calcula as horas trabalhadas: (Saída - Entrada) - (Fim do Intervalo - Início do Intervalo)
        var totalTime = ClockOut.Value - ClockIn.Value;

        var breakTime = TimeSpan.Zero;
        if (BreakStart.HasValue && BreakEnd.HasValue)
        {
            breakTime = BreakEnd.Value - BreakStart.Value;
        }

        var workedTime = totalTime - breakTime;
        var totalHours = (decimal)workedTime.TotalHours;

        return totalHours > 0 ? totalHours : 0m;
    }

    public decimal CalculateCost()
    {
        var hours = Hours ?? CalculateHours();

        // Fórmula: horas * valor-hora * (1 + adicional%) * (1 + encargos%)
        var additionalMultiplier = 1m + (AdditionalSnapshot / 100m);
        var chargesMultiplier = 1m + (ChargesSnapshot / 100m);

        return hours * HourlyRateSnapshot * additionalMultiplier * chargesMultiplier;
    }
}