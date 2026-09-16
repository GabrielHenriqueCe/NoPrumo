using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class EmployeeTraining
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long TrainingTypeId { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? WorkloadHours { get; set; }

    public string? Modality { get; set; }

    public string? Instructor { get; set; }

    public string? CertificateNumber { get; set; }

    public string? AttachmentUrl { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual TrainingType TrainingType { get; set; } = null!;
}
