using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class AccountReceivable
{
    public long Id { get; set; }

    public long? ClientId { get; set; }

    public long? ProjectId { get; set; }

    public string Description { get; set; } = null!;

    public string? InvoiceNumber { get; set; }

    public decimal Amount { get; set; }

    public decimal PaidAmount { get; set; }

    public DateOnly DueDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
