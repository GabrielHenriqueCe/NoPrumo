using NoPrumo.Application.DTOs;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Services;

public sealed class StageService(IStageRepository stageRepository, TimeProvider timeProvider) : IStageService
{
    private const int MaxPageSize = 100;

    public async Task<PagedResult<StageDto>> ListAsync(int page, int size, string? search, long? projectId, CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var stagePage = await stageRepository.ListAsync(page, size, search, projectId, cancellationToken);
        var today = timeProvider.Today();

        var items = stagePage.Items
            .Select(stage => ToDto(stage, today))
            .ToList();

        return new PagedResult<StageDto>(items, stagePage.Page, stagePage.Size, stagePage.Total, stagePage.TotalPages);
    }

    public async Task<StageDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var stage = await stageRepository.GetByIdAsync(id, cancellationToken);
        return stage is null ? null : ToDto(stage, timeProvider.Today());
    }

    private static StageDto ToDto(Stage stage, DateOnly today) => new()
    {
        Id = stage.Id,
        ProjectId = stage.ProjectId,
        ProjectName = stage.Project.Name,
        Name = stage.Name,
        SortOrder = stage.SortOrder,
        TeamId = stage.TeamId,
        TeamName = stage.Team?.Name,
        SupervisorId = stage.SupervisorId,
        SupervisorName = stage.Supervisor?.Name,
        PlannedDate = stage.PlannedDate,
        StartDate = stage.StartDate,
        CompletionDate = stage.CompletionDate,
        Percentage = stage.Percentage,
        Status = stage.Status,
        Notes = stage.Notes,
        Late = stage.IsLate(today)
    };
}
