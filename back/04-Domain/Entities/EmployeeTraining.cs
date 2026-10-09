using System;
using System.Collections.Generic;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Domain.Entities;

public class EmployeeTraining
{
    // Mesma janela da antiga view vw_capacitacoes_alerta: "vence em 30 dias".
    private const int ExpiringWindowDays = 30;

    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long TrainingTypeId { get; set; }

    public DateOnly IssueDate { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int? WorkloadHours { get; set; }

    public TrainingModality? Modality { get; set; }

    public string? Instructor { get; set; }

    public string? CertificateNumber { get; set; }

    public string? AttachmentUrl { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual TrainingType TrainingType { get; set; } = null!;

    public TrainingStatus StatusOn(DateOnly today) => ExpiryDate switch
    {
        null => TrainingStatus.NoExpiry,
        { } expiryDate when expiryDate < today => TrainingStatus.Expired,
        { } expiryDate when expiryDate <= today.AddDays(ExpiringWindowDays) => TrainingStatus.Expiring,
        _ => TrainingStatus.Valid,
    };
}
