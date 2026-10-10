namespace NoPrumo.Application.DTOs;

public sealed record TeamOptionDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string DepartmentName { get; init; }
}
