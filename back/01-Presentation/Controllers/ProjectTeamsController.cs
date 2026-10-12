using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/projects/{projectId:long}/teams")]
[Authorize(Policy = "manage_projects")]
public sealed class ProjectTeamsController(IProjectTeamService projectTeamService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectTeamDto>>> List(long projectId, CancellationToken cancellationToken)
    {
        var allocations = await projectTeamService.ListAsync(projectId, cancellationToken);
        return allocations is null ? NotFound() : Ok(allocations);
    }

    // A tela de equipes pede manage_employees; quem aloca na obra só precisa do nome.
    [HttpGet("options")]
    public async Task<ActionResult<IReadOnlyList<TeamOptionDto>>> ListOptions(long projectId, CancellationToken cancellationToken) =>
        Ok(await projectTeamService.ListTeamOptionsAsync(cancellationToken));

    [HttpPost]
    public async Task<ActionResult<ProjectTeamDto>> Allocate(long projectId, AllocateTeamRequest request, CancellationToken cancellationToken)
    {
        var result = await projectTeamService.AllocateAsync(projectId, request, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return StatusCode(StatusCodes.Status201Created, result.Allocation);
    }

    [HttpPatch("{teamId:long}/end")]
    public async Task<ActionResult<ProjectTeamDto>> End(long projectId, long teamId, EndTeamAllocationRequest request, CancellationToken cancellationToken)
    {
        var result = await projectTeamService.EndAsync(projectId, teamId, request, cancellationToken);
        if (result is null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, detail: "This team is not on this project right now.");
        }

        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return Ok(result.Allocation);
    }

    private ActionResult ValidationProblemFrom(IReadOnlyDictionary<string, string> errors)
    {
        foreach (var (field, message) in errors)
        {
            ModelState.AddModelError(field, message);
        }

        return ValidationProblem(ModelState);
    }
}
