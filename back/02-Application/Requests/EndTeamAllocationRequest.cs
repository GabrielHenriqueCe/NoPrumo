namespace NoPrumo.Application.Requests;

// A equipe sai da obra com data, nunca apagando a linha: é o histórico de
// quem trabalhou em qual obra.
public sealed record EndTeamAllocationRequest
{
    public DateOnly? EndDate { get; init; }
}
