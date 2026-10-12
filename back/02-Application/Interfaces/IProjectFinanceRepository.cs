using NoPrumo.Application.Results;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IProjectFinanceRepository
{
    // Stages e Amendments vêm carregados: as regras do contrato e do avanço precisam deles.
    Task<Project?> GetWithStagesAndAmendmentsAsync(long projectId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Project>> ListActiveWithStagesAndAmendmentsAsync(CancellationToken cancellationToken);

    // Material: entradas na obra, ao custo da nota. Mesmo critério do material-cost do estoque.
    Task<IReadOnlyList<DailyAmount>> ListMaterialCostByDayAsync(long projectId, CancellationToken cancellationToken);

    Task<IReadOnlyList<DailyAmount>> ListLaborCostByDayAsync(long projectId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, decimal>> SumMaterialCostByProjectAsync(CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, decimal>> SumLaborCostByProjectAsync(CancellationToken cancellationToken);
}
