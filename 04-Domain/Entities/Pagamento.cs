using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Pagamento
{
    public long Id { get; set; }

    public long? ContaReceberId { get; set; }

    public long? ContaPagarId { get; set; }

    public decimal Valor { get; set; }

    public DateTime DataPagamento { get; set; }

    public string? FormaPagamento { get; set; }

    public long? RegistradoPor { get; set; }

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ContasPagar? ContaPagar { get; set; }

    public virtual ContasReceber? ContaReceber { get; set; }

    public virtual Usuario? RegistradoPorNavigation { get; set; }
}
