using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public class TrainingType
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? ValidityMonths { get; set; }

    public int? MinWorkloadHours { get; set; }

    public bool RequiresInPerson { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<EmployeeTraining> EmployeeTrainings { get; set; } = new List<EmployeeTraining>();

    // Sem validade (CNH, CREA), o certificado não vence.
    public DateOnly? ExpiryFor(DateOnly issueDate) =>
        ValidityMonths is { } months ? issueDate.AddMonths(months) : null;
}
