using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Funcionario
{
    public long Id { get; set; }

    public string? Matricula { get; set; }

    public string Nome { get; set; } = null!;

    public long? FuncaoId { get; set; }

    public long? RegimeId { get; set; }

    public decimal Valor { get; set; }

    public decimal AdicionalPercentual { get; set; }

    public DateOnly? DataAdmissao { get; set; }

    public DateOnly? DataDemissao { get; set; }

    public string? Telefone { get; set; }

    public byte[]? DocumentoCifrado { get; set; }

    public string? DocumentoHash { get; set; }

    public string? DocumentoMascara { get; set; }

    public bool? Ativo { get; set; }

    public DateTime? AnonimizadoEm { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? AtivoKey { get; set; }

    public virtual ICollection<EstoqueMovimentacao> EstoqueMovimentacoes { get; set; } = new List<EstoqueMovimentacao>();

    public virtual ICollection<Etapa> Etapas { get; set; } = new List<Etapa>();

    public virtual ICollection<Ficha> Fichas { get; set; } = new List<Ficha>();

    public virtual Funcao? Funcao { get; set; }

    public virtual ICollection<FuncionarioCapacitacao> FuncionarioCapacitacoes { get; set; } = new List<FuncionarioCapacitacao>();

    public virtual ICollection<FuncionarioEquipe> FuncionarioEquipes { get; set; } = new List<FuncionarioEquipe>();

    public virtual ICollection<Obra> Obras { get; set; } = new List<Obra>();

    public virtual ICollection<Ponto> Ponto { get; set; } = new List<Ponto>();

    public virtual Regime? Regime { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
