using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class Fichas
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

    public virtual ICollection<FichaItens> FichaItens { get; set; } = new List<FichaItens>();

    public virtual Funcionarios Funcionario { get; set; } = null!;

    public virtual Obras? Obra { get; set; }

    public virtual Usuarios? Responsavel { get; set; }
}
