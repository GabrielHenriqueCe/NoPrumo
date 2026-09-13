using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class ContasReceber
{
    public long Id { get; set; }

    public long? ClienteId { get; set; }

    public long? ObraId { get; set; }

    public string Descricao { get; set; } = null!;

    public string? NumeroNf { get; set; }

    public decimal Valor { get; set; }

    public decimal ValorPago { get; set; }

    public DateOnly Vencimento { get; set; }

    public string Status { get; set; } = null!;

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Clientes? Cliente { get; set; }

    public virtual Obras? Obra { get; set; }

    public virtual ICollection<Pagamentos> Pagamentos { get; set; } = new List<Pagamentos>();
}
