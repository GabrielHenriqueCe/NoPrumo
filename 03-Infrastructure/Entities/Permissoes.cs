using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class Permissoes
{
    public long Id { get; set; }

    public string Chave { get; set; } = null!;

    public string? Descricao { get; set; }

    public virtual ICollection<Papeis> Papel { get; set; } = new List<Papeis>();
}
