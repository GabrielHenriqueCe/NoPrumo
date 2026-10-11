namespace NoPrumo.Application.DTOs.Requests;

public record UpdateJobRoleRequest
{
    public string Name { get; init; } = string.Empty;
    public long DepartmentId { get; init; }
}