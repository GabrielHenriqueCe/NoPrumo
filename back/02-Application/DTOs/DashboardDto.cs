namespace NoPrumo.Application.DTOs;

/// <summary>
/// Painel inicial. Cada número só vem para quem tem a permissão daquele
/// assunto; null quer dizer "não é do seu perfil", e a tela não mostra o cartão.
/// </summary>
public sealed record DashboardDto(
    int? ActiveProjects,
    int? LateProjects,
    int? LateStages,
    int? StockItemsBelowMinimum,
    int? TrainingsNeedingAttention,
    int? UnsignedPpeIssues);
