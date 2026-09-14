using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class UsuarioObra
{
    public long UsuarioId { get; set; }

    public long ObraId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Obra Obra { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
