using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class VwObrasCustos
{
    public long? ObraId { get; set; }

    public string? Codigo { get; set; }

    public string? Nome { get; set; }

    public decimal ContratoAtual { get; set; }

    public decimal CustoMaterial { get; set; }

    public decimal CustoMaoObra { get; set; }

    public decimal CustoEmpreitada { get; set; }
}
