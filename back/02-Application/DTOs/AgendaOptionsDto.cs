namespace NoPrumo.Application.DTOs;

// Só id e nome: a lista de obras inteira tem contrato, e a de usuários pede manage_users.
public sealed record AgendaOptionsDto
{
    public required IReadOnlyList<OptionDto> Projects { get; init; }
    public required IReadOnlyList<OptionDto> Assignees { get; init; }
}
