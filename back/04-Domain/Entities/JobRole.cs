using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class JobRole
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long DepartmentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Department Department { get; set; } = null!;
}
