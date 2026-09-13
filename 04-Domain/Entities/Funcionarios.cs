using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Funcionarios
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

    public virtual ICollection<EstoqueMovimentacoes> EstoqueMovimentacoes { get; set; } = new List<EstoqueMovimentacoes>();

    public virtual ICollection<Etapas> Etapas { get; set; } = new List<Etapas>();

    public virtual ICollection<Fichas> Fichas { get; set; } = new List<Fichas>();

    public virtual Funcoes? Funcao { get; set; }

    public virtual ICollection<FuncionarioCapacitacoes> FuncionarioCapacitacoes { get; set; } = new List<FuncionarioCapacitacoes>();

    public virtual ICollection<FuncionarioEquipes> FuncionarioEquipes { get; set; } = new List<FuncionarioEquipes>();

    public virtual ICollection<Obras> Obras { get; set; } = new List<Obras>();

    public virtual ICollection<Ponto> Ponto { get; set; } = new List<Ponto>();

    public virtual Regimes? Regime { get; set; }

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
}
