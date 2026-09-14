using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Obra
{
    public long Id { get; set; }

    public string Codigo { get; set; } = null!;

    public long? ClienteId { get; set; }

    public string Nome { get; set; } = null!;

    public string? Descricao { get; set; }

    public string? Cno { get; set; }

    public string? Endereco { get; set; }

    public string? Numero { get; set; }

    public string? Complemento { get; set; }

    public string? Bairro { get; set; }

    public string? Cidade { get; set; }

    public string? Estado { get; set; }

    public string? Cep { get; set; }

    public decimal ContratoValor { get; set; }

    public string Status { get; set; } = null!;

    public long? ResponsavelId { get; set; }

    public string? ResponsavelTecnico { get; set; }

    public string? CreaRt { get; set; }

    public DateOnly? DataInicio { get; set; }

    public DateOnly? DataPrevisao { get; set; }

    public DateOnly? DataConclusao { get; set; }

    public DateTime? FechadaEm { get; set; }

    public long? FechadaPor { get; set; }

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? AtivoKey { get; set; }

    public virtual ICollection<Agenda> Agendas { get; set; } = new List<Agenda>();

    public virtual Cliente? Cliente { get; set; }

    public virtual ICollection<ContasPagar> ContasPagar { get; set; } = new List<ContasPagar>();

    public virtual ICollection<ContasReceber> ContasReceber { get; set; } = new List<ContasReceber>();

    public virtual ICollection<ContratosEmpreitada> ContratosEmpreitada { get; set; } = new List<ContratosEmpreitada>();

    public virtual ICollection<EquipeObra> EquipeObras { get; set; } = new List<EquipeObras>();

    public virtual ICollection<EstoqueMovimentacao> EstoqueMovimentacoes { get; set; } = new List<EstoqueMovimentacao>();

    public virtual ICollection<Etapa> Etapas { get; set; } = new List<Etapa>();

    public virtual Usuario? FechadaPorNavigation { get; set; }

    public virtual ICollection<Ficha> Fichas { get; set; } = new List<Ficha>();

    public virtual ICollection<ObraAditivo> ObraAditivos { get; set; } = new List<ObraAditivo>();

    public virtual ICollection<ObraLink> ObraLinks { get; set; } = new List<ObraLink>();

    public virtual ICollection<Ponto> Ponto { get; set; } = new List<Ponto>();

    public virtual Funcionario? Responsavel { get; set; }

    public virtual ICollection<SolicitacaoCompra> SolicitacoesCompra { get; set; } = new List<SolicitacaoCompra>();

    public virtual ICollection<UsuarioObra> UsuarioObras { get; set; } = new List<UsuarioObra>();
}
