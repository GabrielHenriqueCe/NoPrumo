using System;
using System.Collections.Generic;

namespace NoPrumo.Infrastructure.Entities;

public partial class Grupos
{
    public long Id { get; set; }

    public string Nome { get; set; } = null!;

    public long CategoriaId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual CategoriasEstoque Categoria { get; set; } = null!;

    public virtual ICollection<ItensEstoque> ItensEstoque { get; set; } = new List<ItensEstoque>();
}
