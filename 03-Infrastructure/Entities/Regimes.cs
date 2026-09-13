using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class Regimes
{
    public long Id { get; set; }

    public string Rotulo { get; set; } = null!;

    public string Unidade { get; set; } = null!;

    public decimal? HorasMes { get; set; }

    public string? Descricao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Funcionarios> Funcionarios { get; set; } = new List<Funcionarios>();

    public virtual ICollection<ParametrosEncargos> ParametrosEncargos { get; set; } = new List<ParametrosEncargos>();
}
