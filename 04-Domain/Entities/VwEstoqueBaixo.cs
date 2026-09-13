using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class VwEstoqueBaixo
{
    public string Escopo { get; set; } = null!;

    public long? ObraId { get; set; }

    public long ItemId { get; set; }

    public string Item { get; set; } = null!;

    public string Unidade { get; set; } = null!;

    public decimal? Saldo { get; set; }

    public decimal Minimo { get; set; }
}
