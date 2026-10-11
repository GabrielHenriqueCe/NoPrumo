using System.Globalization;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Services;

public sealed class ProjectTeamService(IProjectTeamRepository projectTeamRepository, TimeProvider timeProvider) : IProjectTeamService
{
    public async Task<IReadOnlyList<ProjectTeamDto>?> ListAsync(long projectId, CancellationToken cancellationToken)
    {
        if (!await projectTeamRepository.ProjectExistsAsync(projectId, cancellationToken))
        {
            return null;
        }

        var allocations = await projectTeamRepository.ListByProjectAsync(projectId, cancellationToken);
        var today = timeProvider.Today();

        return allocations.Select(allocation => ToDto(allocation, today)).ToList();
    }

    public async Task<IReadOnlyList<TeamOptionDto>> ListTeamOptionsAsync(CancellationToken cancellationToken)
    {
        var teams = await projectTeamRepository.ListActiveTeamsAsync(cancellationToken);

        return teams
            .Select(team => new TeamOptionDto { Id = team.Id, Name = team.Name, DepartmentName = team.Department.Name })
            .ToList();
    }

    public async Task<SaveProjectTeamResult?> AllocateAsync(long projectId, AllocateTeamRequest request, CancellationToken cancellationToken)
    {
        if (!await projectTeamRepository.ProjectExistsAsync(projectId, cancellationToken))
        {
            return null;
        }

        var errors = new Dictionary<string, string>();

        if (request.StartDate is null)
        {
            errors["startDate"] = "Start date is required.";
        }

        var team = request.TeamId is null
            ? null
            : await projectTeamRepository.GetTeamAsync(request.TeamId.Value, cancellationToken);

        if (!await projectTeamRepository.ActiveProjectExistsAsync(projectId, cancellationToken))
        {
            errors["teamId"] = "This project is inactive. Activate it before allocating teams.";
        }
        else if (team is null)
        {
            errors["teamId"] = "Pick a team.";
        }
        else if (!team.IsActive())
        {
            errors["teamId"] = "This team is inactive.";
        }
        else if (request.StartDate is { } startDate)
        {
            var periods = await projectTeamRepository.ListPeriodsAsync(projectId, team.Id, cancellationToken);

            if (periods.Any(period => period.IsOpen()))
            {
                errors["teamId"] = "This team is already on this project.";
            }
            else if (periods.FirstOrDefault(period => period.Overlaps(startDate)) is { } overlapping)
            {
                errors["startDate"] =
                    $"This team was on this project until {Format(overlapping.EndDate!.Value)}. The new period has to start after that.";
            }
        }

        if (errors.Count > 0)
        {
            return new SaveProjectTeamResult(null, errors);
        }

        var allocation = new TeamProject
        {
            ProjectId = projectId,
            TeamId = team!.Id,
            StartDate = request.StartDate!.Value,
        };

        projectTeamRepository.Add(allocation);
        await projectTeamRepository.SaveChangesAsync(cancellationToken);

        return await ResultAsync(allocation, cancellationToken);
    }

    public async Task<SaveProjectTeamResult?> EndAsync(long projectId, long teamId, EndTeamAllocationRequest request, CancellationToken cancellationToken)
    {
        var allocation = await projectTeamRepository.GetOpenForUpdateAsync(projectId, teamId, cancellationToken);
        if (allocation is null)
        {
            return null;
        }

        var errors = new Dictionary<string, string>();

        if (request.EndDate is null)
        {
            errors["endDate"] = "End date is required.";
        }
        else if (allocation.EndsBeforeStart(request.EndDate.Value))
        {
            errors["endDate"] = $"The end date cannot be before the start date ({Format(allocation.StartDate)}).";
        }

        if (errors.Count > 0)
        {
            return new SaveProjectTeamResult(null, errors);
        }

        allocation.End(request.EndDate!.Value);
        await projectTeamRepository.SaveChangesAsync(cancellationToken);

        return await ResultAsync(allocation, cancellationToken);
    }

    private async Task<SaveProjectTeamResult> ResultAsync(TeamProject allocation, CancellationToken cancellationToken)
    {
        var saved = await projectTeamRepository.GetAsync(allocation.ProjectId, allocation.TeamId, allocation.StartDate, cancellationToken);
        return new SaveProjectTeamResult(ToDto(saved!, timeProvider.Today()), new Dictionary<string, string>());
    }

    private static ProjectTeamDto ToDto(TeamProject allocation, DateOnly today) => new()
    {
        TeamId = allocation.TeamId,
        TeamName = allocation.Team.Name,
        DepartmentName = allocation.Team.Department.Name,
        StartDate = allocation.StartDate,
        EndDate = allocation.EndDate,
        Members = allocation.IsOpen()
            ? allocation.Team.EmployeeTeams
                .Where(membership => membership.EndDate is null && membership.Employee.Active == true)
                .OrderBy(membership => membership.Employee.Name)
                .Select(membership => ToMemberDto(membership.Employee, today))
                .ToList()
            : [],
    };

    private static ProjectTeamMemberDto ToMemberDto(Employee employee, DateOnly today) => new()
    {
        EmployeeId = employee.Id,
        EmployeeName = employee.Name,
        RegistrationNumber = employee.RegistrationNumber,
        TrainingAlerts = employee.LatestTrainings()
            .Where(training => training.NeedsAttentionOn(today))
            .OrderBy(training => training.ExpiryDate)
            .Select(training => new TrainingAlertDto
            {
                TrainingTypeCode = training.TrainingType.Code,
                TrainingTypeName = training.TrainingType.Name,
                ExpiryDate = training.ExpiryDate!.Value,
                Status = training.StatusOn(today),
            })
            .ToList(),
    };

    private static string Format(DateOnly date) =>
        date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
}
