using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class FuncionarioEquipe
{
    public long FuncionarioId { get; set; }

    public long EquipeId { get; set; }

    public DateOnly DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Equipe Equipe { get; set; } = null!;

    public virtual Funcionario Funcionario { get; set; } = null!;
}
