using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class VwSaldoObra
{
    public long? ObraId { get; set; }

    public long ItemId { get; set; }

    public string Item { get; set; } = null!;

    public string Unidade { get; set; } = null!;

    public decimal Minimo { get; set; }

    public decimal? Saldo { get; set; }
}
