using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class Auditoria
{
    public long Id { get; set; }

    public long? UsuarioId { get; set; }

    public string Tabela { get; set; } = null!;

    public long? RegistroId { get; set; }

    public string Acao { get; set; } = null!;

    public string? CamposAlterados { get; set; }

    public string? DadosAnteriores { get; set; }

    public string? DadosNovos { get; set; }

    public string? Ip { get; set; }

    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Usuarios? Usuario { get; set; }
}
