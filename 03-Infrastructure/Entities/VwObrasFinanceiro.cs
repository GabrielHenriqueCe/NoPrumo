using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class VwObrasFinanceiro
{
    public long ObraId { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public decimal ContratoValor { get; set; }

    public decimal TotalAReceber { get; set; }
}
