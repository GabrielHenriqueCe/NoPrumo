using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class ProjectLink
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public string TokenHash { get; set; } = null!;

    public string? Label { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime? LastAccessAt { get; set; }

    public int AccessCount { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual Project Project { get; set; } = null!;
}
