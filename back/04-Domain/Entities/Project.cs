using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Project
{
    public long Id { get; set; }

    public string Code { get; set; } = null!;

    public long? ClientId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Cno { get; set; }

    public string? Street { get; set; }

    public string? Number { get; set; }

    public string? Complement { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public decimal ContractAmount { get; set; }

    public string Status { get; set; } = null!;

    public long? SupervisorId { get; set; }

    public string? TechnicalManager { get; set; }

    public string? CreaRt { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? ForecastDate { get; set; }

    public DateOnly? CompletionDate { get; set; }

    public DateTime? ClosedAt { get; set; }

    public long? ClosedBy { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? ActiveKey { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Client? Client { get; set; }

    public virtual ICollection<AccountPayable> AccountsPayable { get; set; } = new List<AccountPayable>();

    public virtual ICollection<AccountReceivable> AccountsReceivable { get; set; } = new List<AccountReceivable>();

    public virtual ICollection<Subcontract> Subcontracts { get; set; } = new List<Subcontract>();

    public virtual ICollection<TeamProject> TeamProjects { get; set; } = new List<TeamProject>();

    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    public virtual ICollection<Stage> Stages { get; set; } = new List<Stage>();

    public virtual User? ClosedByUser { get; set; }

    public virtual ICollection<PpeIssue> PpeIssues { get; set; } = new List<PpeIssue>();

    public virtual ICollection<ProjectAmendment> Amendments { get; set; } = new List<ProjectAmendment>();

    public virtual ICollection<ProjectLink> Links { get; set; } = new List<ProjectLink>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    public virtual Employee? Supervisor { get; set; }

    public virtual ICollection<PurchaseRequest> PurchaseRequests { get; set; } = new List<PurchaseRequest>();

    public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
}
