namespace NoPrumo.Application.DTOs;

public record PurchaseRequestItemDto
{
    public required long Id { get; init; }
    public required long StockItemId { get; init; }
    public required string StockItemName { get; init; }
    public required decimal RequestedQuantity { get; init; }
    public required decimal FulfilledQuantity { get; init; }
    public required string Unit { get; init; }
    public string? Notes { get; init; }
}
