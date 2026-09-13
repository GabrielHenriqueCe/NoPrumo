using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class FichaItens
{
    public long Id { get; set; }

    public long FichaId { get; set; }

    public long? ItemId { get; set; }

    public string ItemSnapshot { get; set; } = null!;

    public string? CaSnapshot { get; set; }

    public decimal Quantidade { get; set; }

    public string Unidade { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateOnly? DataEntrega { get; set; }

    public DateOnly? DataDevolucao { get; set; }

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Fichas Ficha { get; set; } = null!;

    public virtual ItensEstoque? Item { get; set; }
}
