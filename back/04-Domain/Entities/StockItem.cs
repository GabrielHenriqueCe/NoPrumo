using System;
using System.Collections.Generic;

using NoPrumo.Domain.Enums;

namespace NoPrumo.Domain.Entities;

public class StockItem
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

    public bool IsCaExpiredOn(DateOnly today) => CaExpiryDate < today;

    // Nome e CA são copiados: o comprovante não muda se o cadastro do item mudar.
    public PpeIssueItem IssueAsPpe(decimal quantity, DateOnly issueDate) => new()
    {
        StockItemId = Id,
        ItemNameSnapshot = Name,
        CaSnapshot = Ca,
        Quantity = quantity,
        Unit = Unit,
        Status = PpeItemStatus.Issued,
        IssueDate = issueDate,
    };
}
