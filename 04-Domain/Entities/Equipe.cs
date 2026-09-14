using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Equipe
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public long SetorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? AtivoKey { get; set; }

    public virtual ICollection<EquipeObra> EquipeObras { get; set; } = new List<EquipeObras>();

    public virtual ICollection<Etapas> Etapas { get; set; } = new List<Etapas>();

    public virtual ICollection<FuncionarioEquipes> FuncionarioEquipes { get; set; } = new List<FuncionarioEquipes>();

    public virtual Setores Setor { get; set; } = null!;
}
