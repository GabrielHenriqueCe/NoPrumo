namespace NoPrumo.Application.DTOs;

public record DepartmentDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool Active { get; init; }
}