using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Agenda
{
    public long Id { get; set; }

    public DateOnly Data { get; set; }

    public TimeOnly? Hora { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Descricao { get; set; }

    public long? ObraId { get; set; }

    public long? ResponsavelId { get; set; }

    public string Tipo { get; set; } = null!;

    public bool Concluido { get; set; }

    public DateTime? ConcluidoEm { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Obras? Obra { get; set; }

    public virtual Usuarios? Responsavel { get; set; }
}
