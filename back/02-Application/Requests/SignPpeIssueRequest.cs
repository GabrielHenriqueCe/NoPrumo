namespace NoPrumo.Application.Requests;

public sealed record SignPpeIssueRequest
{
    public string? TypedName { get; init; }
}
