using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class VwEtapasSituacao
{
    public long EtapaId { get; set; }

    public long ObraId { get; set; }

    public string Nome { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal Percentual { get; set; }

    public DateOnly? DataPrevista { get; set; }

    public string Situacao { get; set; } = null!;
}
