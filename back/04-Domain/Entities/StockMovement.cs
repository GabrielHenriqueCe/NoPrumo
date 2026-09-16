using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class StockMovement
{
    public long Id { get; set; }

    public long StockItemId { get; set; }

    public long? ProjectId { get; set; }

    public long? EmployeeId { get; set; }

    public string Type { get; set; } = null!;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = null!;

    public decimal UnitCost { get; set; }

    public long? SupplierId { get; set; }

    public string? InvoiceNumber { get; set; }

    public string? SourceType { get; set; }

    public long? SourceId { get; set; }

    public long? TransferId { get; set; }

    public string? Notes { get; set; }

    public DateTime Date { get; set; }

    public long? RecordedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Supplier? Supplier { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual StockItem StockItem { get; set; } = null!;

    public virtual Project? Project { get; set; }

    public virtual User? RecordedByUser { get; set; }
}
