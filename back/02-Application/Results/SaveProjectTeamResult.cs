using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Results;

public sealed record SaveProjectTeamResult(ProjectTeamDto? Allocation, IReadOnlyDictionary<string, string> Errors)
{
    public bool Succeeded => Errors.Count == 0;
}
