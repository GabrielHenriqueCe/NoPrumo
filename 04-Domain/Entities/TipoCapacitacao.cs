using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class TipoCapacitacao
{
    public long Id { get; set; }

    public string Codigo { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public int? ValidadeMeses { get; set; }

    public int? CargaHorariaMin { get; set; }

    public bool ExigePresencial { get; set; }

    public string? Observacao { get; set; }

    public virtual ICollection<FuncionarioCapacitacao> FuncionarioCapacitacoes { get; set; } = new List<FuncionarioCapacitacao>();
}
