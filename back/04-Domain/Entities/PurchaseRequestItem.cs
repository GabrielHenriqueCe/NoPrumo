using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class PurchaseRequestItem
{
    public long Id { get; set; }

    public long PurchaseRequestId { get; set; }

    public long StockItemId { get; set; }

    public decimal RequestedQuantity { get; set; }

    public decimal FulfilledQuantity { get; set; }

    public string Unit { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual StockItem StockItem { get; set; } = null!;

    public virtual PurchaseRequest PurchaseRequest { get; set; } = null!;
}
