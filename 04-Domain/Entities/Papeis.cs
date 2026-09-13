using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Papeis
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();

    public virtual ICollection<Permissoes> Permissao { get; set; } = new List<Permissoes>();
}
