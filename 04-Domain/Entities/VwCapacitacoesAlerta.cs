using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class VwCapacitacoesAlerta
{
    public long FuncionarioId { get; set; }

    public string Nome { get; set; } = null!;

    public string Codigo { get; set; } = null!;

    public string Capacitacao { get; set; } = null!;

    public DateOnly? DataValidade { get; set; }

    public int? DiasRestantes { get; set; }

    public string Situacao { get; set; } = null!;
}
