using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class StockGroup
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long StockCategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual StockCategory StockCategory { get; set; } = null!;

    public virtual ICollection<StockItem> StockItems { get; set; } = new List<StockItem>();
}
