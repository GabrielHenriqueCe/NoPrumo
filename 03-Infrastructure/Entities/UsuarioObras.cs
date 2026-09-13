using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class UsuarioObras
{
    public long UsuarioId { get; set; }

    public long ObraId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Obras Obra { get; set; } = null!;

    public virtual Usuarios Usuario { get; set; } = null!;
}
