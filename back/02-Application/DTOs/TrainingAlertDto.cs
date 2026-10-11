using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

public sealed record TrainingAlertDto
{
    public required string TrainingTypeCode { get; init; }
    public required string TrainingTypeName { get; init; }
    public required DateOnly ExpiryDate { get; init; }
    public required TrainingStatus Status { get; init; }
}
