using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class MedicaoEmpreitada
{
    public long Id { get; set; }

    public long ContratoId { get; set; }

    public int Numero { get; set; }

    public DateOnly Data { get; set; }

    public decimal? Quantidade { get; set; }

    public decimal? Percentual { get; set; }

    public decimal Valor { get; set; }

    public long? AprovadoPor { get; set; }

    public DateTime? AprovadoEm { get; set; }

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Usuario? AprovadoPorNavigation { get; set; }

    public virtual ContratoEmpreitada Contrato { get; set; } = null!;
}
