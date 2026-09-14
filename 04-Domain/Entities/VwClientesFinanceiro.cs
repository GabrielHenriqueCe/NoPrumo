using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class VwClientesFinanceiro
{
    public long ClienteId { get; set; }

    public string Nome { get; set; } = null!;

    public decimal TotalAReceber { get; set; }
}
