using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Tests.Fakes;

public sealed class FakeStageRepository : IStageRepository
{
    public List<Stage> Stages { get; } = [];

    public Task<PagedResult<Stage>> ListAsync(int page, int size, string? search, long? projectId, CancellationToken cancellationToken)
    {
        var stages = Stages.Where(stage => projectId is null || stage.ProjectId == projectId).ToList();
        return Task.FromResult(new PagedResult<Stage>(stages, page, size, stages.Count, 1));
    }

    public Task<Stage?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        Task.FromResult(Stages.FirstOrDefault(stage => stage.Id == id));

    public Task<Stage?> GetForUpdateAsync(long id, CancellationToken cancellationToken) => throw new NotImplementedException();

    public Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    public void Add(Stage stage) => throw new NotImplementedException();

    public Task SaveChangesAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
}
