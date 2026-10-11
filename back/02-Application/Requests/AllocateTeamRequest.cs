namespace NoPrumo.Application.Requests;

public sealed record AllocateTeamRequest
{
    public long? TeamId { get; init; }
    public DateOnly? StartDate { get; init; }
}
