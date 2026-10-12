using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface ITimeEntryRepository
{
    // Listagem filtrada exigida pelo documento (por obra e período)
    Task<PagedResult<TimeEntry>> ListAsync(int page, int size, long projectId, DateOnly from, DateOnly to, CancellationToken cancellationToken);

    // Consulta de um ponto específico
    Task<TimeEntry?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<decimal> GetTotalLaborCostAsync(long projectId, CancellationToken cancellationToken);

    // Permite buscar as entidades associadas (para cálculos e snapshot) no momento da criação do ponto
    Task<Employee?> GetEmployeeAsync(long employeeId, CancellationToken cancellationToken);
    Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken);

    // O Save padrão exigido na arquitetura
    void Add(TimeEntry timeEntry);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}