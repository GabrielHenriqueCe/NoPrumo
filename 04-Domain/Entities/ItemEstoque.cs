using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class ItemEstoque
{
    public long Id { get; set; }

    public long GrupoId { get; set; }

    public string? Codigo { get; set; }

    public string Item { get; set; } = null!;

    public string Unidade { get; set; } = null!;

    public decimal Minimo { get; set; }

    public decimal PrecoRef { get; set; }

    public string? Ca { get; set; }

    public DateOnly? CaValidade { get; set; }

    public bool? Ativo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? AtivoKey { get; set; }

    public virtual ICollection<EstoqueMovimentacao> EstoqueMovimentacoes { get; set; } = new List<EstoqueMovimentacao>();

    public virtual ICollection<FichaItem> FichaItens { get; set; } = new List<FichaItem>();

    public virtual Grupos Grupo { get; set; } = null!;

    public virtual ICollection<SolicitacaoItem> SolicitacaoItens { get; set; } = new List<SolicitacaoItem>();
}
