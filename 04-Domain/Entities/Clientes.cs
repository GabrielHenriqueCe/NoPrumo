using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Clientes
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public string TipoPessoa { get; set; } = null!;

    public byte[]? DocumentoCifrado { get; set; }

    public string? DocumentoHash { get; set; }

    public string? DocumentoMascara { get; set; }

    public string? Email { get; set; }

    public string? Contato { get; set; }

    public string? Telefone { get; set; }

    public string? Celular { get; set; }

    public string? Endereco { get; set; }

    public string? Numero { get; set; }

    public string? Complemento { get; set; }

    public string? Bairro { get; set; }

    public string? Cidade { get; set; }

    public string? Estado { get; set; }

    public string? Cep { get; set; }

    public string? Observacoes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? AtivoKey { get; set; }

    public virtual ICollection<ContasReceber> ContasReceber { get; set; } = new List<ContasReceber>();

    public virtual ICollection<Obras> Obras { get; set; } = new List<Obras>();
}
