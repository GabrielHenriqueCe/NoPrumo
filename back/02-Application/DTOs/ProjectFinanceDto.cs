using System.Diagnostics.CodeAnalysis;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// Obra para quem tem view_finance: tudo do ProjectDto mais o valor do contrato.
/// </summary>
public sealed record ProjectFinanceDto : ProjectDto
{
    [SetsRequiredMembers]
    public ProjectFinanceDto(ProjectDto project, decimal contractAmount) : base(project)
    {
        ContractAmount = contractAmount;
    }

    public decimal ContractAmount { get; init; }
}
