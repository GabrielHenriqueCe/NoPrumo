namespace NoPrumo.Application.DTOs.Requests;

public record CreateEmployeeRequest
{
    public string? RegistrationNumber { get; init; }
    public string Name { get; init; } = string.Empty;
    public long? JobRoleId { get; init; }
    public long? EmploymentRegimeId { get; init; }
    public decimal PayRate { get; init; }
    public decimal AdditionalPercentage { get; init; }
    public DateOnly? HireDate { get; init; }
    public string? Phone { get; init; }
    public string? Document { get; init; }
}