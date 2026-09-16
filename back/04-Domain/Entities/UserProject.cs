using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class UserProject
{
    public long UserId { get; set; }

    public long ProjectId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
