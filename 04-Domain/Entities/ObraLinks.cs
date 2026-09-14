using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class ObraLinks
{
    public long Id { get; set; }

    public long ObraId { get; set; }

    public string TokenHash { get; set; } = null!;

    public string? Rotulo { get; set; }

    public DateTime? ExpiraEm { get; set; }

    public DateTime? RevogadoEm { get; set; }

    public DateTime? UltimoAcesso { get; set; }

    public int TotalAcessos { get; set; }

    public long? CriadoPor { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Usuarios? CriadoPorNavigation { get; set; }

    public virtual Obras Obra { get; set; } = null!;
}
