using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

public sealed record PpeIssueItemDto
{
    public required long Id { get; init; }
    public required long? StockItemId { get; init; }
    public required string ItemName { get; init; }
    public required string? Ca { get; init; }
    public required decimal Quantity { get; init; }
    public required string Unit { get; init; }
    public required PpeItemStatus Status { get; init; }
    public required DateOnly? IssueDate { get; init; }
    public required DateOnly? ReturnDate { get; init; }
}
