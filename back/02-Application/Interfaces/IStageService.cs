using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;

namespace NoPrumo.Application.Interfaces;

public interface IStageService
{
    Task<PagedResult<StageDto>> ListAsync(int page, int size, string? search, long? projectId, CancellationToken cancellationToken);

    Task<StageDto?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<SaveStageResult> CreateAsync(SaveStageRequest request, CancellationToken cancellationToken);

    Task<SaveStageResult?> UpdateAsync(long id, SaveStageRequest request, CancellationToken cancellationToken);
}
