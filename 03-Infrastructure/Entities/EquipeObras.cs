using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class EquipeObras
{
    public long EquipeId { get; set; }

    public long ObraId { get; set; }

    public DateOnly DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Equipes Equipe { get; set; } = null!;

    public virtual Obras Obra { get; set; } = null!;
}
