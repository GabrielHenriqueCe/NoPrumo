using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Permissao
{
    public long Id { get; set; }

    public string Chave { get; set; } = null!;

    public string? Descricao { get; set; }

    public virtual ICollection<Papel> Papeis { get; set; } = new List<Papel>();
}
