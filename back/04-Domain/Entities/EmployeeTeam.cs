using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class EmployeeTeam
{
    public long EmployeeId { get; set; }

    public long TeamId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
