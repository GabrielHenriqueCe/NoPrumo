using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class ContratoEmpreitada
{
    public long Id { get; set; }

    public long ObraId { get; set; }

    public long? FornecedorId { get; set; }

    public long? EtapaId { get; set; }

    public string Descricao { get; set; } = null!;

    public string TipoPreco { get; set; } = null!;

    public decimal ValorTotal { get; set; }

    public decimal? PrecoUnitario { get; set; }

    public string? Unidade { get; set; }

    public decimal? QuantidadePrevista { get; set; }

    public decimal RetencaoInssPct { get; set; }

    public DateOnly? DataInicio { get; set; }

    public DateOnly? DataFim { get; set; }

    public string Status { get; set; } = null!;

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Etapas? Etapa { get; set; }

    public virtual Fornecedores? Fornecedor { get; set; }

    public virtual ICollection<MedicoesEmpreitada> MedicoesEmpreitada { get; set; } = new List<MedicoesEmpreitada>();

    public virtual Obras Obra { get; set; } = null!;
}
