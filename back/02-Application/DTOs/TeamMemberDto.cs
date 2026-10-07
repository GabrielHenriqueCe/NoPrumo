using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// Uma passagem do funcionário pela equipe (EmployeeTeam). EndDate nulo quer
/// dizer que ele ainda está nela; preenchido, que saiu naquela data.
///
/// Não há Id: a chave da tabela é (funcionário, equipe, data de entrada), e um
/// funcionário tem no máximo uma passagem aberta por equipe — é o EmployeeId
/// que identifica qual encerrar.
/// </summary>
public sealed record TeamMemberDto(
    long EmployeeId,
    string EmployeeName,
    string? RegistrationNumber,
    DateOnly StartDate,
    DateOnly? EndDate)
{
    /// <summary>Exige Employee carregado (.Include(m => m.Employee)).</summary>
    public static TeamMemberDto FromEntity(EmployeeTeam m) => new(
        m.EmployeeId,
        m.Employee.Name,
        m.Employee.RegistrationNumber,
        m.StartDate,
        m.EndDate);
}
