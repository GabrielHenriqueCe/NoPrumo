namespace NoPrumo.Application.Requests;

public sealed record UpdateStockCategoryRequest(
    string Name,
    bool TracksProjectBalance,
    bool RequiresReturn);