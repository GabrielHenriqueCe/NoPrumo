namespace NoPrumo.Application.DTOs.Requests;

public record UpdateDepartmentRequest
{
    public string Name { get; init; } = string.Empty;
}