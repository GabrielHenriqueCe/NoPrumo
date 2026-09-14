using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class FuncionarioEquipes
{
    public long FuncionarioId { get; set; }

    public long EquipeId { get; set; }

    public DateOnly DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Equipes Equipe { get; set; } = null!;

    public virtual Funcionarios Funcionario { get; set; } = null!;
}
