using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class StockCategory
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public bool TracksProjectBalance { get; set; }

    public bool RequiresReturn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<StockGroup> StockGroups { get; set; } = new List<StockGroup>();
}
