using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Client
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string PersonType { get; set; } = null!;

    public byte[]? DocumentEncrypted { get; set; }

    public string? DocumentHash { get; set; }

    public string? DocumentMasked { get; set; }

    public string? Email { get; set; }

    public string? ContactName { get; set; }

    public string? Phone { get; set; }

    public string? Mobile { get; set; }

    public string? Street { get; set; }

    public string? Number { get; set; }

    public string? Complement { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? ActiveKey { get; set; }

    public virtual ICollection<AccountReceivable> AccountsReceivable { get; set; } = new List<AccountReceivable>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
