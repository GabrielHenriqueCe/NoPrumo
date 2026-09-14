using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Funcao
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public long SetorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();

    public virtual Setor Setor { get; set; } = null!;
}
