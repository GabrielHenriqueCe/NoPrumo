using System;

namespace NoPrumo.Application.DTOs.Requests;

public record CreateTimeEntryRequest
{
    public long EmployeeId { get; init; }
    public long ProjectId { get; init; }
    public DateOnly Date { get; init; }
    public string ClockIn { get; init; } = string.Empty;
    public string? BreakStart { get; init; }
    public string? BreakEnd { get; init; }
    public string? ClockOut { get; init; }
    public string? Notes { get; init; }
}