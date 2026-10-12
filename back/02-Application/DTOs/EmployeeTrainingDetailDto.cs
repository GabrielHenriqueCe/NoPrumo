using System;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

public record EmployeeTrainingDetailDto
{
    public long Id { get; init; }
    public string TypeName { get; init; } = string.Empty;
    public DateOnly IssueDate { get; init; }
    public DateOnly? ExpiryDate { get; init; }
    public TrainingStatus Status { get; init; }
}