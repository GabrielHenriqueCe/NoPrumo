using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public class StockCategory
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public bool TracksProjectBalance { get; set; }

    public bool RequiresReturn { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<StockGroup> StockGroups { get; set; } = new List<StockGroup>();

    // EPI é a categoria sem saldo por obra e sem devolução: as duas flags definem
    // os três fluxos de estoque. Mesmo critério do StockMovementsController.
    public bool IsPpe() => !TracksProjectBalance && !RequiresReturn;
}
