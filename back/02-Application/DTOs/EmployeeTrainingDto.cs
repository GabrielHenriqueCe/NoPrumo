using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// Um treinamento na ficha do funcionário (NR-35, ASO...).
///
/// ExpiryDate e Status saem sempre da API, nunca do front: o vencimento é a
/// emissão mais a validade do tipo, calculado ao salvar; o status compara esse
/// vencimento com a data de hoje (EmployeeTraining.StatusOn).
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
    TrainingStatus Status,
    int? WorkloadHours,
    TrainingModality? Modality,
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
        t.StatusOn(today),
        t.WorkloadHours,
        t.Modality,
        t.Instructor);
}

// ExpiryDate não existe no request: o vencimento é conta da API. Ids e datas
// chegam anuláveis para a validação ser do controller, com a mensagem no
// campo certo.
public sealed record CreateEmployeeTrainingRequest(
    long? EmployeeId,
    long? TrainingTypeId,
    DateOnly? IssueDate,
    int? WorkloadHours,
    TrainingModality? Modality,
    string? Instructor);

public sealed record UpdateEmployeeTrainingRequest(
    long? EmployeeId,
    long? TrainingTypeId,
    DateOnly? IssueDate,
    int? WorkloadHours,
    TrainingModality? Modality,
    string? Instructor);
