using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Results;

public sealed record SaveStageResult(StageDto? Stage, IReadOnlyDictionary<string, string> Errors)
{
    public bool Succeeded => Errors.Count == 0;
}