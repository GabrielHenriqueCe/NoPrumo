using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class SolicitacoesCompra
{
    public long Id { get; set; }

    public long ObraId { get; set; }

    public long? SolicitadoPor { get; set; }

    public DateOnly Data { get; set; }

    public DateOnly? DataNecessidade { get; set; }

    public string Status { get; set; } = null!;

    public string? Observacao { get; set; }

    public long? DecididoPor { get; set; }

    public DateTime? DecididoEm { get; set; }

    public string? MotivoRecusa { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Usuarios? DecididoPorNavigation { get; set; }

    public virtual Obras Obra { get; set; } = null!;

    public virtual ICollection<SolicitacaoItens> SolicitacaoItens { get; set; } = new List<SolicitacaoItens>();

    public virtual Usuarios? SolicitadoPorNavigation { get; set; }
}
