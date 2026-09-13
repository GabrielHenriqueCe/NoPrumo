using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class Funcoes
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public long SetorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Funcionarios> Funcionarios { get; set; } = new List<Funcionarios>();

    public virtual Setores Setor { get; set; } = null!;
}
