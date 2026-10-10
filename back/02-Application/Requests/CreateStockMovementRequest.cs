using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.Requests;

// ProjectId é obrigatório só para itens de categoria com saldo por obra
// (consumable material); EmployeeId só para categoria com devolução (tool).
// O controller decide qual é obrigatório olhando a categoria do item.
public sealed record CreateStockMovementRequest(
    long StockItemId,
    StockMovementType Type,
    decimal Quantity,
    long? ProjectId,
    long? EmployeeId,
    string? Notes);