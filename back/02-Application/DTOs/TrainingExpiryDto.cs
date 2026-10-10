using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

// Vencimento calculado, nunca gravado como compromisso.
public sealed record TrainingExpiryDto
{
    public required long TrainingId { get; init; }
    public required long EmployeeId { get; init; }
    public required string EmployeeName { get; init; }
    public required string TrainingTypeCode { get; init; }
    public required string TrainingTypeName { get; init; }
    public required DateOnly ExpiryDate { get; init; }
    public required TrainingStatus Status { get; init; }
}
