using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

// ExpiryDate e Status saem sempre da API: o front nunca calcula vencimento.
public sealed record EmployeeTrainingDto
{
    public required long Id { get; init; }
    public required long EmployeeId { get; init; }
    public required string EmployeeName { get; init; }
    public required string? RegistrationNumber { get; init; }
    public required long TrainingTypeId { get; init; }
    public required string TrainingTypeCode { get; init; }
    public required string TrainingTypeName { get; init; }
    public required DateOnly IssueDate { get; init; }
    public required DateOnly? ExpiryDate { get; init; }
    public required TrainingStatus Status { get; init; }
    public required int? WorkloadHours { get; init; }
    public required TrainingModality? Modality { get; init; }
    public required string? Instructor { get; init; }
}
