using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Projects;
using NoPrumo.Application.Interfaces;
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
        var today = Today();

        var items = projectPage.Items
            .Select(project => ToDto(project, today, canViewFinance))
            .ToList();

        return new PagedResult<ProjectDto>(items, projectPage.Page, projectPage.Size, projectPage.Total, projectPage.TotalPages);
    }

    public async Task<ProjectDto?> GetByIdAsync(long id, bool canViewFinance, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(id, cancellationToken);
        return project is null ? null : ToDto(project, Today(), canViewFinance);
    }

    // Data local, não UTC: depois das 21h em Brasília o UTC já virou o dia
    // e a obra apareceria atrasada um dia antes.
    private DateOnly Today() => DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);

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