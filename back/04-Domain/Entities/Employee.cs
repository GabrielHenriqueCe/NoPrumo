using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Employee
{
    public long Id { get; set; }

    public string? RegistrationNumber { get; set; }

    public string Name { get; set; } = null!;

    public long? JobRoleId { get; set; }

    public long? EmploymentRegimeId { get; set; }

    public decimal PayRate { get; set; }

    public decimal AdditionalPercentage { get; set; }

    public DateOnly? HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public string? Phone { get; set; }

    public byte[]? DocumentEncrypted { get; set; }

    public string? DocumentHash { get; set; }

    public string? DocumentMasked { get; set; }

    public bool? Active { get; set; }

    public DateTime? AnonymizedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? ActiveKey { get; set; }

    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    public virtual ICollection<Stage> Stages { get; set; } = new List<Stage>();

    public virtual ICollection<PpeIssue> PpeIssues { get; set; } = new List<PpeIssue>();

    public virtual JobRole? JobRole { get; set; }

    public virtual ICollection<EmployeeTraining> EmployeeTrainings { get; set; } = new List<EmployeeTraining>();

    public virtual ICollection<EmployeeTeam> EmployeeTeams { get; set; } = new List<EmployeeTeam>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    public virtual EmploymentRegime? EmploymentRegime { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
