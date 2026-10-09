namespace NoPrumo.Application.DTOs;

/// <summary>
/// O grupo de estoque como o front o recebe.
/// </summary>
public sealed record StockGroupDto(
    long Id,
    string Name,
    long StockCategoryId,
    string StockCategoryName);