using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class ParametrosEncargos
{
    public long Id { get; set; }

    public long RegimeId { get; set; }

    public decimal Percentual { get; set; }

    public DateOnly VigenciaInicio { get; set; }

    public DateOnly? VigenciaFim { get; set; }

    public string Fonte { get; set; } = null!;

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Regimes Regime { get; set; } = null!;
}
