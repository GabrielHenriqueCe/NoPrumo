using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class SolicitacaoItem
{
    public long Id { get; set; }

    public long SolicitacaoId { get; set; }

    public long ItemId { get; set; }

    public decimal QtdSolicitada { get; set; }

    public decimal QtdAtendida { get; set; }

    public string Unidade { get; set; } = null!;

    public string? Observacao { get; set; }

    public virtual ItemEstoque Item { get; set; } = null!;

    public virtual SolicitacaoCompra Solicitacao { get; set; } = null!;
}
