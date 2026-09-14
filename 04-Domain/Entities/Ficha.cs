using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Ficha
{
    public long Id { get; set; }

    public long FuncionarioId { get; set; }

    public long? ObraId { get; set; }

    public DateOnly Data { get; set; }

    public string? Observacao { get; set; }

    public long? ResponsavelId { get; set; }

    public DateTime? AssinadoEm { get; set; }

    public string? AssinaturaUrl { get; set; }

    public string? AssinaturaHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<FichaItem> FichaItens { get; set; } = new List<FichaItem>();

    public virtual Funcionarios Funcionario { get; set; } = null!;

    public virtual Obra? Obra { get; set; }

    public virtual Usuario? Responsavel { get; set; }
}
