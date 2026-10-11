using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Responses;

namespace NoPrumo.Controllers;

[ApiController]
[Authorize(Policy = "manage_projects")]
public sealed class ProjectLinksController(IProjectLinkService projectLinkService) : ControllerBase
{
    private long? CurrentUserId
    {
        get
        {
            var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return long.TryParse(subject, out var id) ? id : null;
        }
    }

    [HttpPost("api/projects/{projectId:long}/links")]
    public async Task<ActionResult<CreateProjectLinkResponse>> Create(
        long projectId,
        CreateProjectLinkRequest request,
        CancellationToken cancellationToken)
    {
        var response = await projectLinkService.CreateAsync(projectId, request, CurrentUserId, cancellationToken);
        if (response is null)
        {
            return NotFound(new { message = "Project not found." });
        }

        return Ok(response);
    }

    [HttpGet("api/projects/{projectId:long}/links")]
    public async Task<ActionResult<IReadOnlyList<ProjectLinkDto>>> ListByProject(
        long projectId,
        CancellationToken cancellationToken)
    {
        var links = await projectLinkService.ListByProjectIdAsync(projectId, cancellationToken);
        return Ok(links);
    }

    [HttpPatch("api/project-links/{id:long}/revoke")]
    public async Task<IActionResult> Revoke(long id, CancellationToken cancellationToken)
    {
        var revoked = await projectLinkService.RevokeAsync(id, cancellationToken);
        if (!revoked)
        {
            return NotFound(new { message = "Link not found or already revoked." });
        }

        return NoContent();
    }
}
