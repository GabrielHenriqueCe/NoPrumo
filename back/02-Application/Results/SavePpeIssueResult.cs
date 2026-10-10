using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Results;

public sealed record SavePpeIssueResult(PpeIssueDto? Issue, IReadOnlyDictionary<string, string> Errors)
{
    public bool Succeeded => Errors.Count == 0;
}
