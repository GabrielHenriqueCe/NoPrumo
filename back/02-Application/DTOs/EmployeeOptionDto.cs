namespace NoPrumo.Application.DTOs;

public sealed record EmployeeOptionDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required string? RegistrationNumber { get; init; }
}
