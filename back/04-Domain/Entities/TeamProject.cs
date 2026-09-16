using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class TeamProject
{
    public long TeamId { get; set; }

    public long ProjectId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
