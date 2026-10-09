namespace NoPrumo.Application.Requests;

public sealed record CreateStockGroupRequest(string Name, long StockCategoryId);