namespace NoPrumo.Application.Requests;

public sealed record SaveTeamRequest
{
    public string? Name { get; init; }
    public long? DepartmentId { get; init; }
}
