namespace NoPrumo.Application.DTOs;

public class EmploymentRegimeDto
{
    public long Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal? MonthlyHours { get; set; }
    public string? Description { get; set; }
    public bool Active { get; set; }
}

public class CreateEmploymentRegimeRequest
{
    public string Label { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal? MonthlyHours { get; set; }
    public string? Description { get; set; }
}

public class UpdateEmploymentRegimeRequest
{
    public string Label { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal? MonthlyHours { get; set; }
    public string? Description { get; set; }
    public bool Active { get; set; }
}