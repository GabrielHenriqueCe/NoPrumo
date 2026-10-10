using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// Uma movimentação de estoque como o front a recebe. Quantity é sempre
/// positiva: o sinal (soma ou subtrai do saldo) vem de Type, nunca do número.
/// </summary>
public sealed record StockMovementDto(
    long Id,
    long StockItemId,
    string StockItemName,
    StockMovementType Type,
    decimal Quantity,
    string Unit,
    long? ProjectId,
    string? ProjectName,
    long? EmployeeId,
    string? EmployeeName,
    string? Notes,
    DateTime Date);