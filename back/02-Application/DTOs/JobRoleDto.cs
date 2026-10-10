namespace NoPrumo.Application.DTOs;

public record JobRoleDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public long DepartmentId { get; init; }
    public bool Active { get; init; }
}