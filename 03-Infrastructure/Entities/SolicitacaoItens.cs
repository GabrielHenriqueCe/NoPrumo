using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class SolicitacaoItens
{
    public long Id { get; set; }

    public long SolicitacaoId { get; set; }

    public long ItemId { get; set; }

    public decimal QtdSolicitada { get; set; }

    public decimal QtdAtendida { get; set; }

    public string Unidade { get; set; } = null!;

    public string? Observacao { get; set; }

    public virtual ItensEstoque Item { get; set; } = null!;

    public virtual SolicitacoesCompra Solicitacao { get; set; } = null!;
}
