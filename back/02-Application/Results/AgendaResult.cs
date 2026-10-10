using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Results;

public sealed record AgendaResult(AgendaDto? Agenda, IReadOnlyDictionary<string, string> Errors)
{
    public bool Succeeded => Errors.Count == 0;
}
