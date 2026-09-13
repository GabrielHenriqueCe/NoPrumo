using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class ObraAditivos
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

    public virtual Usuarios? AprovadoPorNavigation { get; set; }

    public virtual Obras Obra { get; set; } = null!;
}
