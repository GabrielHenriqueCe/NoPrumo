using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Usuario
{
    public long Id { get; set; }

    public string Usuario { get; set; } = null!;

    public string SenhaHash { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string? Email { get; set; }

    public long PapelId { get; set; }

    public long? FuncionarioId { get; set; }

    public bool? Ativo { get; set; }

    public DateTime? UltimoLogin { get; set; }

    public int TentativasFalhas { get; set; }

    public DateTime? BloqueadoAte { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Agenda> Agendas { get; set; } = new List<Agenda>();

    public virtual ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();

    public virtual ICollection<EstoqueMovimentacao> EstoqueMovimentacoes { get; set; } = new List<EstoqueMovimentacoes>();

    public virtual ICollection<Etapa> Etapas { get; set; } = new List<Etapas>();

    public virtual ICollection<Ficha> Fichas { get; set; } = new List<Fichas>();

    public virtual Funcionario? Funcionario { get; set; }

    public virtual ICollection<MedicoesEmpreitada> MedicoesEmpreitadas { get; set; } = new List<MedicoesEmpreitada>();

    public virtual ICollection<ObraAditivo> ObraAditivos { get; set; } = new List<ObraAditivos>();

    public virtual ICollection<ObraLink> ObraLinks { get; set; } = new List<ObraLinks>();

    public virtual ICollection<Obra> Obras { get; set; } = new List<Obras>();

    public virtual ICollection<Pagamento> Pagamentos { get; set; } = new List<Pagamentos>();

    public virtual Papel Papeis { get; set; } = null!;

    public virtual ICollection<Ponto> Pontos { get; set; } = new List<Ponto>();

    public virtual ICollection<SolicitacaoCompra> SolicitacoesCompraDecididoPorNavigation { get; set; } = new List<SolicitacoesCompra>();

    public virtual ICollection<SolicitacaoCompra> SolicitacoesCompraSolicitadoPorNavigation { get; set; } = new List<SolicitacoesCompra>();

    public virtual ICollection<UsuarioObra> UsuarioObras { get; set; } = new List<UsuarioObras>();
}
