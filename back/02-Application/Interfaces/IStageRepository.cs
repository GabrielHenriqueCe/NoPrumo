using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IStageRepository
{
    Task<PagedResult<Stage>> ListAsync(int page, int size, string? search, long? projectId, CancellationToken cancellationToken);

    Task<Stage?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<Stage?> GetForUpdateAsync(long id, CancellationToken cancellationToken);

    Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken);

    void Add(Stage stage);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
