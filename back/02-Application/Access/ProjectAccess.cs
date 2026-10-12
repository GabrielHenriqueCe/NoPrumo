namespace NoPrumo.Application.Access;

/// <summary>
/// Quais obras a pessoa enxerga. Quem tem view_all_projects vê todas; o mestre
/// vê só as que supervisiona (Project.SupervisorId == funcionário ligado ao usuário).
/// Usuário sem funcionário ligado e sem view_all_projects não vê nenhuma.
/// </summary>
public sealed record ProjectAccess(bool SeesAllProjects, long? EmployeeId)
{
    public static ProjectAccess AllProjects { get; } = new(true, null);
}
