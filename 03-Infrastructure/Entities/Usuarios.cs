using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class Usuarios
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

    public virtual ICollection<Agenda> Agenda { get; set; } = new List<Agenda>();

    public virtual ICollection<Auditoria> Auditoria { get; set; } = new List<Auditoria>();

    public virtual ICollection<EstoqueMovimentacoes> EstoqueMovimentacoes { get; set; } = new List<EstoqueMovimentacoes>();

    public virtual ICollection<Etapas> Etapas { get; set; } = new List<Etapas>();

    public virtual ICollection<Fichas> Fichas { get; set; } = new List<Fichas>();

    public virtual Funcionarios? Funcionario { get; set; }

    public virtual ICollection<MedicoesEmpreitada> MedicoesEmpreitada { get; set; } = new List<MedicoesEmpreitada>();

    public virtual ICollection<ObraAditivos> ObraAditivos { get; set; } = new List<ObraAditivos>();

    public virtual ICollection<ObraLinks> ObraLinks { get; set; } = new List<ObraLinks>();

    public virtual ICollection<Obras> Obras { get; set; } = new List<Obras>();

    public virtual ICollection<Pagamentos> Pagamentos { get; set; } = new List<Pagamentos>();

    public virtual Papeis Papel { get; set; } = null!;

    public virtual ICollection<Ponto> Ponto { get; set; } = new List<Ponto>();

    public virtual ICollection<SolicitacoesCompra> SolicitacoesCompraDecididoPorNavigation { get; set; } = new List<SolicitacoesCompra>();

    public virtual ICollection<SolicitacoesCompra> SolicitacoesCompraSolicitadoPorNavigation { get; set; } = new List<SolicitacoesCompra>();

    public virtual ICollection<UsuarioObras> UsuarioObras { get; set; } = new List<UsuarioObras>();
}
