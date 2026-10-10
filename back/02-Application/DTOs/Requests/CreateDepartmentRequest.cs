namespace NoPrumo.Application.DTOs.Requests;

public record CreateDepartmentRequest
{
    public string Name { get; init; } = string.Empty;
}