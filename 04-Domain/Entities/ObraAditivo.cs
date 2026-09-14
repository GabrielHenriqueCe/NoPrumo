using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class ObraAditivo
{
    public long Id { get; set; }

    public long ObraId { get; set; }

    public string Tipo { get; set; } = null!;

    public string? Numero { get; set; }

    public DateOnly Data { get; set; }

    public decimal Valor { get; set; }

    public int DiasPrazo { get; set; }

    public string? Motivo { get; set; }

    public long? AprovadoPor { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Usuario? AprovadoPorNavigation { get; set; }

    public virtual Obra Obra { get; set; } = null!;
}
