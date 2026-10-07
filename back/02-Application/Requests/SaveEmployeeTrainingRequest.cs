using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.Requests;

// Não há ExpiryDate: o vencimento é conta da API (TrainingType.ExpiryFor).
public sealed record SaveEmployeeTrainingRequest
{
    public long? EmployeeId { get; init; }
    public long? TrainingTypeId { get; init; }
    public DateOnly? IssueDate { get; init; }
    public int? WorkloadHours { get; init; }
    public TrainingModality? Modality { get; init; }
    public string? Instructor { get; init; }
}
