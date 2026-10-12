using System.Security.Cryptography;
using System.Text;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Responses;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.Services;

public sealed class ProjectLinkService(
    IProjectLinkRepository projectLinkRepository,
    IProjectRepository projectRepository,
    TimeProvider timeProvider) : IProjectLinkService
{
    private const int DefaultValidityDays = 90;

    public async Task<CreateProjectLinkResponse?> CreateAsync(
        long projectId,
        CreateProjectLinkRequest request,
        long? createdByUserId,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project is null)
        {
            return null;
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var daysValid = request.DaysValid.HasValue && request.DaysValid.Value > 0
            ? request.DaysValid.Value
            : DefaultValidityDays;

        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');

        var tokenHash = ComputeSha256Hash(token);

        var link = new ProjectLink
        {
            ProjectId = projectId,
            TokenHash = tokenHash,
            Label = request.Label?.Trim(),
            ExpiresAt = now.AddDays(daysValid),
            RevokedAt = null,
            AccessCount = 0,
            LastAccessAt = null,
            CreatedBy = createdByUserId,
            CreatedAt = now
        };

        projectLinkRepository.Add(link);
        await projectLinkRepository.SaveChangesAsync(cancellationToken);

        return new CreateProjectLinkResponse(
            link.Id,
            link.ProjectId,
            token,
            link.Label,
            link.ExpiresAt,
            link.CreatedAt);
    }

    public async Task<bool> RevokeAsync(long id, CancellationToken cancellationToken)
    {
        var link = await projectLinkRepository.GetByIdAsync(id, cancellationToken);
        if (link is null)
        {
            return false;
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (!link.Revoke(now))
        {
            return false;
        }

        await projectLinkRepository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ProjectLinkDto>> ListByProjectIdAsync(long projectId, CancellationToken cancellationToken)
    {
        var links = await projectLinkRepository.ListByProjectIdAsync(projectId, cancellationToken);
        var now = timeProvider.GetUtcNow().UtcDateTime;

        return links
            .Select(link => new ProjectLinkDto(
                link.Id,
                link.ProjectId,
                link.Label,
                link.ExpiresAt,
                link.RevokedAt,
                link.LastAccessAt,
                link.AccessCount,
                link.CreatedAt,
                link.IsValid(now)))
            .ToList();
    }

    public async Task<PortalProjectDto?> GetPortalProjectAsync(string token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var tokenHash = ComputeSha256Hash(token.Trim());
        var link = await projectLinkRepository.GetByHashAsync(tokenHash, cancellationToken);
        if (link is null)
        {
            return null;
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (!link.IsValid(now))
        {
            return null;
        }

        link.RecordAccess(now);
        await projectLinkRepository.SaveChangesAsync(cancellationToken);

        var project = link.Project;
        var today = timeProvider.Today();

        var stages = project.Stages
            .OrderBy(stage => stage.SortOrder)
            .Select(stage => new PortalStageDto(
                stage.Name,
                stage.Percentage,
                stage.Status.ToString(),
                stage.IsLate(today)))
            .ToList();

        var progressPercentage = project.Status == ProjectStatus.Completed
            ? 100m
            : stages.Count > 0
                ? Math.Round(stages.Average(stage => stage.Percentage), 1)
                : 0m;

        return new PortalProjectDto(
            project.Name,
            project.City,
            project.Status.ToString(),
            progressPercentage,
            project.ForecastDate,
            stages);
    }

    public static string ComputeSha256Hash(string rawToken)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
