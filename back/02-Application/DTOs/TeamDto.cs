namespace NoPrumo.Application.DTOs;

/// <summary>
/// A equipe como o front a recebe. Active sai de DeletedAt — a tabela não tem
/// coluna active. MemberCount conta só as passagens abertas (sem EndDate):
/// quem já saiu fica no histórico, não na equipe.
/// </summary>
public sealed record TeamDto(
    long Id,
    string Name,
    long DepartmentId,
    string DepartmentName,
    bool Active,
    int MemberCount);
