namespace NoPrumo.Application.Requests;

public sealed record AddTeamMemberRequest
{
    public long? EmployeeId { get; init; }
    public DateOnly? StartDate { get; init; }
}
