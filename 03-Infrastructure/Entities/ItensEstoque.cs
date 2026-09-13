using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class ItensEstoque
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

    public virtual ICollection<EstoqueMovimentacoes> EstoqueMovimentacoes { get; set; } = new List<EstoqueMovimentacoes>();

    public virtual ICollection<FichaItens> FichaItens { get; set; } = new List<FichaItens>();

    public virtual Grupos Grupo { get; set; } = null!;

    public virtual ICollection<SolicitacaoItens> SolicitacaoItens { get; set; } = new List<SolicitacaoItens>();
}
