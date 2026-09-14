using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Ponto
{
    public long Id { get; set; }

    public long FuncionarioId { get; set; }

    public long ObraId { get; set; }

    public DateOnly Data { get; set; }

    public TimeOnly? HoraEntrada { get; set; }

    public TimeOnly? HoraSaidaIntervalo { get; set; }

    public TimeOnly? HoraVoltaIntervalo { get; set; }

    public TimeOnly? HoraSaida { get; set; }

    public decimal? Horas { get; set; }

    public decimal ValorHoraSnapshot { get; set; }

    public decimal AdicionalSnapshot { get; set; }

    public decimal EncargosSnapshot { get; set; }

    public decimal Custo { get; set; }

    public string Origem { get; set; } = null!;

    public string? IdExterno { get; set; }

    public long? RegistradoPor { get; set; }

    public string? Observacao { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Funcionarios Funcionario { get; set; } = null!;

    public virtual Obras Obra { get; set; } = null!;

    public virtual Usuarios? RegistradoPorNavigation { get; set; }
}
