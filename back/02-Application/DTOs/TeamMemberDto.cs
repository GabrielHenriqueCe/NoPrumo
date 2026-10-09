namespace NoPrumo.Application.DTOs;

// Não há Id: a chave de employee_team é (funcionário, equipe, entrada), e cada
// funcionário tem no máximo uma passagem aberta por equipe.
public sealed record TeamMemberDto
{
    public required long EmployeeId { get; init; }
    public required string EmployeeName { get; init; }
    public required string? RegistrationNumber { get; init; }
    public required DateOnly StartDate { get; init; }
    public required DateOnly? EndDate { get; init; }
}
