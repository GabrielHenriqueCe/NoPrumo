namespace NoPrumo.Application.DTOs;

public record EmployeeDto
{
    public long Id { get; init; }
    public string? RegistrationNumber { get; init; }
    public string Name { get; init; } = string.Empty;
    public long? JobRoleId { get; init; }
    public string? JobRoleName { get; init; }
    public long? EmploymentRegimeId { get; init; }
    public string? EmploymentRegimeLabel { get; init; }
    public DateOnly? HireDate { get; init; }
    public string? Phone { get; init; }
    public string? DocumentMasked { get; init; }
    public bool Active { get; init; }
}