namespace NoPrumo.Application.DTOs;

// Active sai de DeletedAt (a tabela não tem coluna active). MemberCount conta só
// as passagens abertas: quem já saiu fica no histórico, não na equipe.
public sealed record TeamDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
    public required long DepartmentId { get; init; }
    public required string DepartmentName { get; init; }
    public required bool Active { get; init; }
    public required int MemberCount { get; init; }
}
