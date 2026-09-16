using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Department
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();

    public virtual ICollection<JobRole> JobRoles { get; set; } = new List<JobRole>();
}
