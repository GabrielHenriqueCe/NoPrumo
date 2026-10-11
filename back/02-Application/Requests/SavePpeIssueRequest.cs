namespace NoPrumo.Application.Requests;

public sealed record SavePpeIssueRequest
{
    public long? EmployeeId { get; init; }
    public long? ProjectId { get; init; }
    public DateOnly? Date { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<PpeIssueItemRequest>? Items { get; init; }
}
