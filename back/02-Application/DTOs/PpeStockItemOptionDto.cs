namespace NoPrumo.Application.DTOs;

// Sem ReferencePrice: quem entrega EPI enxerga quantidade, não dinheiro.
public sealed record PpeStockItemOptionDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string Unit { get; init; }
    public required string? Ca { get; init; }
    public required DateOnly? CaExpiryDate { get; init; }
    public required bool CaExpired { get; init; }
}
