using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Papel
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    public virtual ICollection<Permissao> Permissoes { get; set; } = new List<Permissao>();
}
