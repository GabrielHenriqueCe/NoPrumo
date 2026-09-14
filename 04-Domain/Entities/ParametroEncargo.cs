using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class ParametroEncargo
{
    public long Id { get; set; }

    public long RegimeId { get; set; }

    public decimal Percentual { get; set; }

    public DateOnly VigenciaInicio { get; set; }

    public DateOnly? VigenciaFim { get; set; }

    public string Fonte { get; set; } = null!;

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Regime Regime { get; set; } = null!;
}
