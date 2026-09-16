using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class AuditLog
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string TableName { get; set; } = null!;

    public long? RecordId { get; set; }

    public string Action { get; set; } = null!;

    public string? ChangedFields { get; set; }

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public string? Ip { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
