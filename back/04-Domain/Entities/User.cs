using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class User
{
    public long Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public long RoleId { get; set; }

    public long? EmployeeId { get; set; }

    public bool? Active { get; set; }

    public bool MustChangePassword { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public int FailedAttempts { get; set; }

    public DateTime? LockedUntil { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    public virtual ICollection<Stage> Stages { get; set; } = new List<Stage>();

    public virtual ICollection<PpeIssue> PpeIssues { get; set; } = new List<PpeIssue>();

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<SubcontractMeasurement> SubcontractMeasurements { get; set; } = new List<SubcontractMeasurement>();

    public virtual ICollection<ProjectAmendment> ProjectAmendments { get; set; } = new List<ProjectAmendment>();

    public virtual ICollection<ProjectLink> ProjectLinks { get; set; } = new List<ProjectLink>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    public virtual ICollection<PurchaseRequest> PurchaseRequestsDecided { get; set; } = new List<PurchaseRequest>();

    public virtual ICollection<PurchaseRequest> PurchaseRequestsRequested { get; set; } = new List<PurchaseRequest>();

    public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
}
