using NoPrumo.Application.Access;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;

namespace NoPrumo.Application.Services;

public sealed class StageService(
    IStageRepository stageRepository,
    IProjectRepository projectRepository,
    TimeProvider timeProvider) : IStageService
{
    private const int MaxPageSize = 100;

    public async Task<PagedResult<StageDto>> ListAsync(int page, int size, string? search, long? projectId, ProjectAccess access, CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        // Quem não vê todas as obras só lista etapas de uma obra dele, nunca a lista geral.
        if (!access.SeesAllProjects
            && (projectId is null || await projectRepository.GetByIdAsync(projectId.Value, access, cancellationToken) is null))
        {
            return new PagedResult<StageDto>([], page, size, 0, 1);
        }

        var stagePage = await stageRepository.ListAsync(page, size, search, projectId, cancellationToken);
        var today = timeProvider.Today();

        var items = stagePage.Items
            .Select(stage => ToDto(stage, today))
            .ToList();

        return new PagedResult<StageDto>(items, stagePage.Page, stagePage.Size, stagePage.Total, stagePage.TotalPages);
    }

    public async Task<StageDto?> GetByIdAsync(long id, ProjectAccess access, CancellationToken cancellationToken)
    {
        var stage = await stageRepository.GetByIdAsync(id, cancellationToken);
        if (stage is null)
        {
            return null;
        }

        // Etapa de obra que não é dele: 404, como se não existisse.
        if (!access.SeesAllProjects && await projectRepository.GetByIdAsync(stage.ProjectId, access, cancellationToken) is null)
        {
            return null;
        }

        return ToDto(stage, timeProvider.Today());
    }

    public async Task<SaveStageResult> CreateAsync(SaveStageRequest request, CancellationToken cancellationToken)
    {
        var stage = new Stage();
        Apply(request, stage);

        var errors = await ValidateAsync(stage, cancellationToken);
        if (errors.Count > 0)
        {
            return new SaveStageResult(null, errors);
        }

        stageRepository.Add(stage);
        await stageRepository.SaveChangesAsync(cancellationToken);

        return new SaveStageResult(await GetByIdAsync(stage.Id, ProjectAccess.AllProjects, cancellationToken), errors);
    }

    public async Task<SaveStageResult?> UpdateAsync(long id, SaveStageRequest request, CancellationToken cancellationToken)
    {
        var stage = await stageRepository.GetForUpdateAsync(id, cancellationToken);
        if (stage is null)
        {
            return null;
        }

        Apply(request, stage);

        var errors = await ValidateAsync(stage, cancellationToken);
        if (errors.Count > 0)
        {
            return new SaveStageResult(null, errors);
        }

        await stageRepository.SaveChangesAsync(cancellationToken);

        return new SaveStageResult(await GetByIdAsync(id, ProjectAccess.AllProjects, cancellationToken), errors);
    }

    private async Task<Dictionary<string, string>> ValidateAsync(Stage stage, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string>();

        if (!await stageRepository.ActiveProjectExistsAsync(stage.ProjectId, cancellationToken))
        {
            errors["projectId"] = "Pick an active project.";
        }

        if (stage.IsCompletedWithPartialProgress())
        {
            errors["percentage"] = "A completed stage must be at 100%.";
        }

        return errors;
    }

    private static void Apply(SaveStageRequest request, Stage stage)
    {
        stage.ProjectId = request.ProjectId;
        stage.Name = request.Name.Trim();
        stage.SortOrder = request.SortOrder;
        stage.TeamId = request.TeamId;
        stage.SupervisorId = request.SupervisorId;
        stage.PlannedDate = request.PlannedDate;
        stage.Percentage = request.Percentage;
        stage.Status = request.Status;
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
