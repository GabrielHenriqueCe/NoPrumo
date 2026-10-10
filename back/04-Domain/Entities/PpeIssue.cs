using System;
using System.Collections.Generic;
using System.Linq;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Domain.Entities;

public class PpeIssue
{
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long? ProjectId { get; set; }

    public DateOnly Date { get; set; }

    public string? Notes { get; set; }

    public long? IssuedById { get; set; }

    public DateTime? SignedAt { get; set; }

    public string? SignatureUrl { get; set; }

    public string? SignatureHash { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<PpeIssueItem> Items { get; set; } = new List<PpeIssueItem>();

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project? Project { get; set; }

    public virtual User? IssuedBy { get; set; }

    private const string StockSourceType = "ppe_issue";

    public bool IsSigned() => SignedAt is not null;

    public void Sign(DateTime signedAt, string signatureHash)
    {
        SignedAt = signedAt;
        SignatureHash = signatureHash;
    }

    // Cada item entregue é uma saída do depósito para o funcionário. Exige Id:
    // a ficha já precisa estar gravada.
    public IEnumerable<StockMovement> StockExits(DateTime now, long? recordedBy) =>
        Items.Select(item => new StockMovement
        {
            StockItemId = item.StockItemId!.Value,
            ProjectId = ProjectId,
            EmployeeId = EmployeeId,
            Type = StockMovementType.Exit,
            Quantity = item.Quantity,
            Unit = item.Unit,
            SourceType = StockSourceType,
            SourceId = Id,
            Notes = $"PPE issue #{Id}",
            Date = now,
            RecordedBy = recordedBy,
        });
}
