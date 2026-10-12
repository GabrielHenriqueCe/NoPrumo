using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public class EmploymentRegime
{
    public long Id { get; set; }

    public string Label { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public decimal? MonthlyHours { get; set; }

    public string? Description { get; set; }

    public bool Active {get;set;} = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<PayrollChargeRate> PayrollChargeRates { get; set; } = new List<PayrollChargeRate>();

    // Jornada CLT: 220 h/mês quando o regime mensal não informa, 8 h/dia para diarista.
    private const decimal DefaultMonthlyHours = 220m;
    private const decimal DailyHours = 8m;

    // O PayRate do funcionário está na unidade do regime: salário do mês, diária ou valor-hora.
    public decimal HourlyRate(decimal payRate) => Unit switch
    {
        "month" => payRate / (MonthlyHours ?? DefaultMonthlyHours),
        "day" => payRate / DailyHours,
        _ => payRate,
    };
}
