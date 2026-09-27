using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// O grupo de estoque como o front o recebe.
/// </summary>
public sealed record StockGroupDto(
    long Id,
    string Name,
    long StockCategoryId,
    string StockCategoryName)
{
    /// <summary>
    /// Exige que o StockGroup venha com a StockCategory carregada
    /// (.Include(g => g.StockCategory)), senão g.StockCategory.Name quebra.
    /// </summary>
    public static StockGroupDto FromEntity(StockGroup g) => new(
        g.Id,
        g.Name,
        g.StockCategoryId,
        g.StockCategory.Name);
}

public sealed record CreateStockGroupRequest(string Name, long StockCategoryId);

public sealed record UpdateStockGroupRequest(string Name, long StockCategoryId);