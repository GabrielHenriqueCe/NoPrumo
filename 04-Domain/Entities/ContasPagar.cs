using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class ContasPagar
{
    public long Id { get; set; }

    public long? FornecedorId { get; set; }

    public long? ObraId { get; set; }

    public string Descricao { get; set; } = null!;

    public string? NumeroNf { get; set; }

    public decimal Valor { get; set; }

    public decimal ValorPago { get; set; }

    public DateOnly Vencimento { get; set; }

    public string Status { get; set; } = null!;

    public string? OrigemTipo { get; set; }

    public long? OrigemId { get; set; }

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Fornecedor? Fornecedor { get; set; }

    public virtual Obra? Obra { get; set; }

    public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamento>();
}
