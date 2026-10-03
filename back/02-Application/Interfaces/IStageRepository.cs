using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IStageRepository
{
    Task<PagedResult<Stage>> ListAsync(int page, int size, string? search, long? projectId, CancellationToken cancellationToken);

    Task<Stage?> GetByIdAsync(long id, CancellationToken cancellationToken);
}