namespace NoPrumo.Application.DTOs;

// Members só vem preenchido no período aberto: é quem está na equipe hoje.
public sealed record ProjectTeamDto
{
    public required long TeamId { get; init; }
    public required string TeamName { get; init; }
    public required string DepartmentName { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly? EndDate { get; init; }
    public required IReadOnlyList<ProjectTeamMemberDto> Members { get; init; }
}
