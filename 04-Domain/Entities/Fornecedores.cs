using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Fornecedores
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public byte[]? DocumentoCifrado { get; set; }

    public string? DocumentoHash { get; set; }

    public string? DocumentoMascara { get; set; }

    public string? Contato { get; set; }

    public string? Telefone { get; set; }

    public string? Email { get; set; }

    public string? Cidade { get; set; }

    public string? Estado { get; set; }

    public string? Observacoes { get; set; }

    public bool? Ativo { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? AtivoKey { get; set; }

    public virtual ICollection<ContasPagar> ContasPagar { get; set; } = new List<ContasPagar>();

    public virtual ICollection<ContratosEmpreitada> ContratosEmpreitada { get; set; } = new List<ContratosEmpreitada>();

    public virtual ICollection<EstoqueMovimentacoes> EstoqueMovimentacoes { get; set; } = new List<EstoqueMovimentacoes>();
}
