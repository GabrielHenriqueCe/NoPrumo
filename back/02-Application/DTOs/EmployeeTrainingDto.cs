using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// Um treinamento na ficha do funcionário (NR-35, ASO...).
///
/// ExpiryDate e Status saem sempre da API, nunca do front: o vencimento é a
/// emissão mais a validade do tipo, calculado ao salvar; o status compara esse
/// vencimento com a data de hoje (ver TrainingStatus).
/// </summary>
public sealed record EmployeeTrainingDto(
    long Id,
    long EmployeeId,
    string EmployeeName,
    string? RegistrationNumber,
    long TrainingTypeId,
    string TrainingTypeCode,
    string TrainingTypeName,
    DateOnly IssueDate,
    DateOnly? ExpiryDate,
    string Status,
    int? WorkloadHours,
    string? Modality,
    string? Instructor)
{
    /// <summary>
    /// Exige Employee e TrainingType carregados
    /// (.Include(t => t.Employee).Include(t => t.TrainingType)).
    /// </summary>
    public static EmployeeTrainingDto FromEntity(EmployeeTraining t, DateOnly today) => new(
        t.Id,
        t.EmployeeId,
        t.Employee.Name,
        t.Employee.RegistrationNumber,
        t.TrainingTypeId,
        t.TrainingType.Code,
        t.TrainingType.Name,
        t.IssueDate,
        t.ExpiryDate,
        TrainingStatus.Of(t.ExpiryDate, today),
        t.WorkloadHours,
        t.Modality,
        t.Instructor);
}

/// <summary>
/// A situação do certificado na data de hoje. Não é gravada em coluna
/// nenhuma — é a regra da antiga view vw_capacitacoes_alerta (vencida, vence
/// em 30 dias, ok), agora em C#.
/// </summary>
public static class TrainingStatus
{
    public const string Valid = "valid";
    public const string Expiring = "expiring";
    public const string Expired = "expired";
    public const string NoExpiry = "no_expiry";

    /// <summary>A partir de quantos dias antes do vencimento o certificado pede atenção.</summary>
    public const int ExpiringWindowDays = 30;

    public static string Of(DateOnly? expiryDate, DateOnly today) => expiryDate switch
    {
        null => NoExpiry,
        { } date when date < today => Expired,
        { } date when date <= today.AddDays(ExpiringWindowDays) => Expiring,
        _ => Valid,
    };
}

/// <summary>Os valores aceitos em Modality — gravados em inglês, como todo valor do banco.</summary>
public static class TrainingModality
{
    public const string InPerson = "in_person";
    public const string Online = "online";
    public const string Blended = "blended";

    public static readonly IReadOnlySet<string> All = new HashSet<string> { InPerson, Online, Blended };
}

// ExpiryDate não existe no request: o vencimento é conta da API. Ids e datas
// chegam anuláveis para a validação ser do controller, com a mensagem no
// campo certo.
public sealed record CreateEmployeeTrainingRequest(
    long? EmployeeId,
    long? TrainingTypeId,
    DateOnly? IssueDate,
    int? WorkloadHours,
    string? Modality,
    string? Instructor);

public sealed record UpdateEmployeeTrainingRequest(
    long? EmployeeId,
    long? TrainingTypeId,
    DateOnly? IssueDate,
    int? WorkloadHours,
    string? Modality,
    string? Instructor);
