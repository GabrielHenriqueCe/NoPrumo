using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class EstoqueMovimentacoes
{
    public long Id { get; set; }

    public long ItemId { get; set; }

    public long? ObraId { get; set; }

    public long? FuncionarioId { get; set; }

    public string Tipo { get; set; } = null!;

    public decimal Quantidade { get; set; }

    public string Unidade { get; set; } = null!;

    public decimal CustoUnitario { get; set; }

    public long? FornecedorId { get; set; }

    public string? NumeroNf { get; set; }

    public string? OrigemTipo { get; set; }

    public long? OrigemId { get; set; }

    public long? TransferenciaId { get; set; }

    public string? Observacao { get; set; }

    public DateTime Data { get; set; }

    public long? RegistradoPor { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Fornecedores? Fornecedor { get; set; }

    public virtual Funcionarios? Funcionario { get; set; }

    public virtual ItensEstoque Item { get; set; } = null!;

    public virtual Obras? Obra { get; set; }

    public virtual Usuarios? RegistradoPorNavigation { get; set; }
}
