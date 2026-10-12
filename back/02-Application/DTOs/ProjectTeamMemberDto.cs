namespace NoPrumo.Application.DTOs;

public sealed record ProjectTeamMemberDto
{
    public required long EmployeeId { get; init; }
    public required string EmployeeName { get; init; }
    public required string? RegistrationNumber { get; init; }
    public required IReadOnlyList<TrainingAlertDto> TrainingAlerts { get; init; }
}
