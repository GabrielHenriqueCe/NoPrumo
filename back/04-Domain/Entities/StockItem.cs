using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class StockItem
{
    public long Id { get; set; }

    public long StockGroupId { get; set; }

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public decimal MinQuantity { get; set; }

    public decimal ReferencePrice { get; set; }

    public string? Ca { get; set; }

    public DateOnly? CaExpiryDate { get; set; }

    public bool? Active { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? ActiveKey { get; set; }

    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    public virtual ICollection<PpeIssueItem> PpeIssueItems { get; set; } = new List<PpeIssueItem>();

    public virtual StockGroup StockGroup { get; set; } = null!;

    public virtual ICollection<PurchaseRequestItem> PurchaseRequestItems { get; set; } = new List<PurchaseRequestItem>();
}
