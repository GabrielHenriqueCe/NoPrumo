using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// O tipo de treinamento como o front o recebe (NR-35, NR-10, ASO...).
/// ValidityMonths nulo quer dizer que o certificado não vence;
/// MinWorkloadHours nulo, que não há carga mínima.
/// </summary>
public sealed record TrainingTypeDto(
    long Id,
    string Code,
    string Name,
    int? ValidityMonths,
    int? MinWorkloadHours,
    bool RequiresInPerson)
{
    public static TrainingTypeDto FromEntity(TrainingType t) => new(
        t.Id,
        t.Code,
        t.Name,
        t.ValidityMonths,
        t.MinWorkloadHours,
        t.RequiresInPerson);
}

// Texto chega como string? para a validação ser do controller, com a
// mensagem no campo certo — senão o [ApiController] responde antes, com o
// texto padrão do .NET.
public sealed record CreateTrainingTypeRequest(
    string? Code,
    string? Name,
    int? ValidityMonths,
    int? MinWorkloadHours,
    bool RequiresInPerson);

public sealed record UpdateTrainingTypeRequest(
    string? Code,
    string? Name,
    int? ValidityMonths,
    int? MinWorkloadHours,
    bool RequiresInPerson);
