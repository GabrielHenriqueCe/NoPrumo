namespace NoPrumo.Application.DTOs.Requests;

public record CreateEmploymentRegimeRequest
{
    public string Label { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public decimal? MonthlyHours { get; init; }
    public string? Description { get; init; }
}