using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class EmploymentRegime
{
    public long Id { get; set; }

    public string Label { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public decimal? MonthlyHours { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<PayrollChargeRate> PayrollChargeRates { get; set; } = new List<PayrollChargeRate>();
}
