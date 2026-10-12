using System;

namespace NoPrumo.Application.DTOs;

public record TimeEntryDto
{
    public long Id { get; init; }
    public long EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public long ProjectId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly? ClockIn { get; init; }
    public TimeOnly? BreakStart { get; init; }
    public TimeOnly? BreakEnd { get; init; }
    public TimeOnly? ClockOut { get; init; }
    public decimal? Hours { get; init; }
    public decimal? Cost { get; init; } // Nulável para ser omitido por segurança
    public string Source { get; init; } = string.Empty;
    public string? Notes { get; init; }
}