using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Stages;

namespace NoPrumo.Application.Interfaces;

public interface IStageService
{
    Task<PagedResult<StageDto>> ListAsync(int page, int size, string? search, long? projectId, CancellationToken cancellationToken);

    Task<StageDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
}