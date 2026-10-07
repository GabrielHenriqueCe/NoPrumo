namespace NoPrumo.Application.DTOs;

// ValidityMonths nulo: o certificado não vence. MinWorkloadHours nulo: não há carga mínima.
public sealed record TrainingTypeDto
{
    public required long Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required int? ValidityMonths { get; init; }
    public required int? MinWorkloadHours { get; init; }
    public required bool RequiresInPerson { get; init; }
}
