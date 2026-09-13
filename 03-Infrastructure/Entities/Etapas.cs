using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class Etapas
{
    public long Id { get; set; }

    public long ObraId { get; set; }

    public string Nome { get; set; } = null!;

    public int Ordem { get; set; }

    public long? EquipeId { get; set; }

    public long? ResponsavelId { get; set; }

    public DateOnly? DataPrevista { get; set; }

    public DateOnly? DataInicio { get; set; }

    public DateOnly? DataConclusao { get; set; }

    public decimal Percentual { get; set; }

    public string Status { get; set; } = null!;

    public string? Observacao { get; set; }

    public long? MarcadoPor { get; set; }

    public DateTime? MarcadoEm { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ContratosEmpreitada> ContratosEmpreitada { get; set; } = new List<ContratosEmpreitada>();

    public virtual Equipes? Equipe { get; set; }

    public virtual Usuarios? MarcadoPorNavigation { get; set; }

    public virtual Obras Obra { get; set; } = null!;

    public virtual Funcionarios? Responsavel { get; set; }
}
