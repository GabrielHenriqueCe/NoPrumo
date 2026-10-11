namespace NoPrumo.Application.DTOs;

public record EmployeeFinancialDto : EmployeeDto
{
    public decimal PayRate { get; init; }
    public decimal AdditionalPercentage { get; init; }
}