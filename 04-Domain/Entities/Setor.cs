using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Setor
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Equipe> Equipes { get; set; } = new List<Equipe>();

    public virtual ICollection<Funcao> Funcoes { get; set; } = new List<Funcao>();
}
