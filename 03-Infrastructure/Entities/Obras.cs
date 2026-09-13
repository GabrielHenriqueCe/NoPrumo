using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class Obras
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

    public string? Observacoes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? AtivoKey { get; set; }

    public virtual ICollection<Agenda> Agenda { get; set; } = new List<Agenda>();

    public virtual Clientes? Cliente { get; set; }

    public virtual ICollection<ContasPagar> ContasPagar { get; set; } = new List<ContasPagar>();

    public virtual ICollection<ContasReceber> ContasReceber { get; set; } = new List<ContasReceber>();

    public virtual ICollection<ContratosEmpreitada> ContratosEmpreitada { get; set; } = new List<ContratosEmpreitada>();

    public virtual ICollection<EquipeObras> EquipeObras { get; set; } = new List<EquipeObras>();

    public virtual ICollection<EstoqueMovimentacoes> EstoqueMovimentacoes { get; set; } = new List<EstoqueMovimentacoes>();

    public virtual ICollection<Etapas> Etapas { get; set; } = new List<Etapas>();

    public virtual Usuarios? FechadaPorNavigation { get; set; }

    public virtual ICollection<Fichas> Fichas { get; set; } = new List<Fichas>();

    public virtual ICollection<ObraAditivos> ObraAditivos { get; set; } = new List<ObraAditivos>();

    public virtual ICollection<ObraLinks> ObraLinks { get; set; } = new List<ObraLinks>();

    public virtual ICollection<Ponto> Ponto { get; set; } = new List<Ponto>();

    public virtual Funcionarios? Responsavel { get; set; }

    public virtual ICollection<SolicitacoesCompra> SolicitacoesCompra { get; set; } = new List<SolicitacoesCompra>();

    public virtual ICollection<UsuarioObras> UsuarioObras { get; set; } = new List<UsuarioObras>();
}
