using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public partial class Payment
{
    public long Id { get; set; }

    public long? AccountReceivableId { get; set; }

    public long? AccountPayableId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public long? RecordedBy { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AccountPayable? AccountPayable { get; set; }

    public virtual AccountReceivable? AccountReceivable { get; set; }

    public virtual User? RecordedByUser { get; set; }
}
