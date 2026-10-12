using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

/// <summary>Uma linha do relatório de fechamento: os números do Finance para cada obra.</summary>
public sealed record ProjectClosingDto(
    long ProjectId,
    string Code,
    string Name,
    ProjectStatus Status,
    bool Late,
    decimal RevisedContractAmount,
    decimal MaterialCost,
    decimal LaborCost,
    decimal TotalCost,
    decimal Margin,
    decimal SpentPercentage,
    decimal ProgressPercentage,
    bool CostAheadOfProgress);
