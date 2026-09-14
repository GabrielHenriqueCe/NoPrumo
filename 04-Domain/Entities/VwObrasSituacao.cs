using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class VwObrasSituacao
{
    public long? ObraId { get; set; }

    public string? Codigo { get; set; }

    public string? Nome { get; set; }

    public string? Status { get; set; }

    public DateOnly? DataPrevisao { get; set; }

    public DateTime? FechadaEm { get; set; }

    public string Situacao { get; set; } = null!;

    public long? TotalEtapas { get; set; }

    public long? EtapasConcluidas { get; set; }
}
