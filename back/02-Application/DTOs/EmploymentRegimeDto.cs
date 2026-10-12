namespace NoPrumo.Application.DTOs;

public record EmploymentRegimeDto
{
    public long Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal? MonthlyHours { get; init; }
    public string? Description { get; init; }
    public bool Active { get; init; }
}