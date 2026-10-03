using NoPrumo.Application.DTOs.Projects;

namespace NoPrumo.Application.Results.Projects;

public sealed record SaveProjectResult(ProjectDto? Project, IReadOnlyDictionary<string, string> Errors)
{
    public bool Succeeded => Errors.Count == 0;
}