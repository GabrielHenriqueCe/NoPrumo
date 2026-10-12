namespace NoPrumo.Application.DTOs;

public record PurchaseRequestDto
{
    public required long Id { get; init; }
    public required long ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public long? RequestedBy { get; init; }
    public string? RequesterName { get; init; }
    public required DateOnly Date { get; init; }
    public DateOnly? NeededByDate { get; init; }
    public required string Status { get; init; }
    public string? Notes { get; init; }
    public long? DecidedBy { get; init; }
    public string? DeciderName { get; init; }
    public DateTime? DecidedAt { get; init; }
    public string? RejectionReason { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required IReadOnlyList<PurchaseRequestItemDto> Items { get; init; }
}
