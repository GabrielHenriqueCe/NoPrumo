namespace NoPrumo.Application.Requests;

public sealed record PpeIssueItemRequest
{
    public long? StockItemId { get; init; }
    public decimal? Quantity { get; init; }
}
