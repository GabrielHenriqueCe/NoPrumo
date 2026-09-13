using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Setores
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Equipes> Equipes { get; set; } = new List<Equipes>();

    public virtual ICollection<Funcoes> Funcoes { get; set; } = new List<Funcoes>();
}
