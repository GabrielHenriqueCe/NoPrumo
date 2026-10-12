using System;

namespace NoPrumo.Application.DTOs.Requests;

public record CreateTimeEntryRequest
{
    public long EmployeeId { get; init; }
    public long ProjectId { get; init; }
    public DateOnly Date { get; init; }
    public TimeOnly ClockIn { get; init; }
    public TimeOnly? BreakStart { get; init; }
    public TimeOnly? BreakEnd { get; init; }
    public TimeOnly? ClockOut { get; init; }
    public string? Notes { get; init; }
}