namespace NoPrumo.Application.DTOs;

/// <summary>
/// Aba Finance da obra (só view_finance). Gasto e avanço em % do contrato
/// revisado: com a obra em andamento, a margem sozinha engana.
/// </summary>
public sealed record ProjectFinancialsDto(
    long ProjectId,
    decimal ContractAmount,
    decimal AmendmentsAmount,
    decimal RevisedContractAmount,
    decimal MaterialCost,
    decimal LaborCost,
    decimal TotalCost,
    decimal SpentPercentage,
    decimal ProgressPercentage,
    bool CostAheadOfProgress,
    decimal Margin,
    IReadOnlyList<DailyCostDto> CostByDay);
