namespace NoPrumo.Application.DTOs;

public sealed record PpeIssueOptionsDto
{
    public required IReadOnlyList<EmployeeOptionDto> Employees { get; init; }
    public required IReadOnlyList<PpeStockItemOptionDto> Items { get; init; }
}
