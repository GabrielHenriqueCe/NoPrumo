using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Regime
{
    public long Id { get; set; }

    public string Rotulo { get; set; } = null!;

    public string Unidade { get; set; } = null!;

    public decimal? HorasMes { get; set; }

    public string? Descricao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Funcionario> Funcionarios { get; set; } = new List<Funcionario>();

    public virtual ICollection<ParametroEncargo> ParametrosEncargos { get; set; } = new List<ParametroEncargo>();
}
