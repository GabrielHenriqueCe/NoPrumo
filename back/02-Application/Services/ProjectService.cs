using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Projects;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests.Projects;
using NoPrumo.Application.Results.Projects;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Services;

public sealed class ProjectService(IProjectRepository projectRepository, TimeProvider timeProvider) : IProjectService
{
    private const int MaxPageSize = 100;

    public async Task<PagedResult<ProjectDto>> ListAsync(int page, int size, string? search, bool canViewFinance, CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var projectPage = await projectRepository.ListAsync(page, size, search, cancellationToken);
        var today = timeProvider.Today();

        var items = projectPage.Items
            .Select(project => ToDto(project, today, canViewFinance))
            .ToList();

        return new PagedResult<ProjectDto>(items, projectPage.Page, projectPage.Size, projectPage.Total, projectPage.TotalPages);
    }

    public async Task<ProjectDto?> GetByIdAsync(long id, bool canViewFinance, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(id, cancellationToken);
        return project is null ? null : ToDto(project, timeProvider.Today(), canViewFinance);
    }

    public async Task<SaveProjectResult> CreateAsync(SaveProjectRequest request, bool canViewFinance, CancellationToken cancellationToken)
    {
        var project = new Project();
        Apply(request, project, canViewFinance);

        var errors = await ValidateAsync(project, null, cancellationToken);
        if (errors.Count > 0)
        {
            return new SaveProjectResult(null, errors);
        }

        projectRepository.Add(project);
        await projectRepository.SaveChangesAsync(cancellationToken);

        return new SaveProjectResult(await GetByIdAsync(project.Id, canViewFinance, cancellationToken), errors);
    }

    public async Task<SaveProjectResult?> UpdateAsync(long id, SaveProjectRequest request, bool canViewFinance, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetForUpdateAsync(id, cancellationToken);
        if (project is null)
        {
            return null;
        }

        Apply(request, project, canViewFinance);

        var errors = await ValidateAsync(project, id, cancellationToken);
        if (errors.Count > 0)
        {
            return new SaveProjectResult(null, errors);
        }

        await projectRepository.SaveChangesAsync(cancellationToken);

        return new SaveProjectResult(await GetByIdAsync(id, canViewFinance, cancellationToken), errors);
    }

    public async Task<bool> ActivateAsync(long id, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetForUpdateAsync(id, cancellationToken);
        if (project is null)
        {
            return false;
        }

        project.Activate();
        await projectRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeactivateAsync(long id, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetForUpdateAsync(id, cancellationToken);
        if (project is null)
        {
            return false;
        }

        project.Deactivate(timeProvider.Now());
        await projectRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task<Dictionary<string, string>> ValidateAsync(Project project, long? ignoredProjectId, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string>();

        if (await projectRepository.CodeExistsAsync(project.Code, ignoredProjectId, cancellationToken))
        {
            errors["code"] = "Another project already uses this code.";
        }

        if (project.IsForecastBeforeStart())
        {
            errors["forecastDate"] = "The forecast date cannot be before the start date.";
        }

        return errors;
    }

    private static void Apply(SaveProjectRequest request, Project project, bool canViewFinance)
    {
        project.Code = request.Code.Trim();
        project.Name = request.Name.Trim();
        project.ClientId = request.ClientId;
        project.Status = request.Status;
        project.SupervisorId = request.SupervisorId;
        project.Cno = request.Cno;
        project.TechnicalManager = request.TechnicalManager;
        project.CreaRt = request.CreaRt;
        project.StartDate = request.StartDate;
        project.ForecastDate = request.ForecastDate;
        project.Street = request.Street;
        project.Number = request.Number;
        project.Complement = request.Complement;
        project.District = request.District;
        project.City = request.City;
        project.State = request.State?.ToUpperInvariant();
        project.PostalCode = request.PostalCode;

        if (canViewFinance && request.ContractAmount is decimal contractAmount)
        {
            project.ContractAmount = contractAmount;
        }
    }

    private static ProjectDto ToDto(Project project, DateOnly today, bool canViewFinance)
    {
        var projectDto = new ProjectDto
        {
            Id = project.Id,
            Code = project.Code,
            Name = project.Name,
            Description = project.Description,
            ClientId = project.ClientId,
            ClientName = project.Client?.Name,
            Cno = project.Cno,
            Street = project.Street,
            Number = project.Number,
            Complement = project.Complement,
            District = project.District,
            City = project.City,
            State = project.State,
            PostalCode = project.PostalCode,
            Status = project.Status,
            SupervisorId = project.SupervisorId,
            SupervisorName = project.Supervisor?.Name,
            TechnicalManager = project.TechnicalManager,
            CreaRt = project.CreaRt,
            StartDate = project.StartDate,
            ForecastDate = project.ForecastDate,
            CompletionDate = project.CompletionDate,
            Notes = project.Notes,
            Late = project.IsLate(today),
            Active = project.DeletedAt is null
        };

        return canViewFinance
            ? new ProjectFinanceDto(projectDto, project.ContractAmount)
            : projectDto;
    }
}