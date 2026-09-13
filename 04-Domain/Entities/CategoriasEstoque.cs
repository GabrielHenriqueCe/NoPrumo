using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class CategoriasEstoque
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public bool ControlaSaldoObra { get; set; }

    public bool ExigeDevolucao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Grupos> Grupos { get; set; } = new List<Grupos>();
}
