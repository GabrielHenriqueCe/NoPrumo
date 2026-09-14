using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class FuncionarioCapacitacoes
{
    public long Id { get; set; }

    public long FuncionarioId { get; set; }

    public long TipoId { get; set; }

    public DateOnly DataEmissao { get; set; }

    public DateOnly? DataValidade { get; set; }

    public int? CargaHoraria { get; set; }

    public string? Modalidade { get; set; }

    public string? Instrutor { get; set; }

    public string? NumeroCertificado { get; set; }

    public string? AnexoUrl { get; set; }

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Funcionarios Funcionario { get; set; } = null!;

    public virtual TiposCapacitacao Tipo { get; set; } = null!;
}
