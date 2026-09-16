using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Supplier
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public byte[]? DocumentEncrypted { get; set; }

    public string? DocumentHash { get; set; }

    public string? DocumentMasked { get; set; }

    public string? ContactName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Notes { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? ActiveKey { get; set; }

    public virtual ICollection<AccountPayable> AccountsPayable { get; set; } = new List<AccountPayable>();

    public virtual ICollection<Subcontract> Subcontracts { get; set; } = new List<Subcontract>();

    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
