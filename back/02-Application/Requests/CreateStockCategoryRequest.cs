namespace NoPrumo.Application.Requests;

public sealed record CreateStockCategoryRequest(
	string Name,
	bool TracksProjectBalance,
	bool RequiresReturn);