using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Team
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long DepartmentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? ActiveKey { get; set; }

    public virtual ICollection<TeamProject> TeamProjects { get; set; } = new List<TeamProject>();

    public virtual ICollection<Stage> Stages { get; set; } = new List<Stage>();

    public virtual ICollection<EmployeeTeam> EmployeeTeams { get; set; } = new List<EmployeeTeam>();

    public virtual Department Department { get; set; } = null!;
}
