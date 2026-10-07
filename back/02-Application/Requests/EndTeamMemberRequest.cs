namespace NoPrumo.Application.Requests;

// Membro sai da equipe com data, nunca apagando a linha: apagar destruiria o
// histórico de quem estava onde.
public sealed record EndTeamMemberRequest
{
    public DateOnly? EndDate { get; init; }
}
