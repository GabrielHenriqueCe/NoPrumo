using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class EquipeObra
{
    public long EquipeId { get; set; }

    public long ObraId { get; set; }

    public DateOnly DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Equipe Equipe { get; set; } = null!;

    public virtual Obras Obra { get; set; } = null!;
}
