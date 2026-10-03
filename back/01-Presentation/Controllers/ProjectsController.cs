using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Projects;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests.Projects;
using NoPrumo.Application.Results.Projects;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_projects")]
public sealed class ProjectsController(IProjectService projectService) : ControllerBase
{
    private bool CanViewFinance => User.HasClaim("permission", "view_finance");

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProjectDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default) =>
        Ok(await projectService.ListAsync(page, size, search, CanViewFinance, cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProjectDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var project = await projectService.GetByIdAsync(id, CanViewFinance, cancellationToken);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(SaveProjectRequest request, CancellationToken cancellationToken)
    {
        var result = await projectService.CreateAsync(request, CanViewFinance, cancellationToken);
        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Project!.Id }, result.Project);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ProjectDto>> Update(long id, SaveProjectRequest request, CancellationToken cancellationToken)
    {
        var result = await projectService.UpdateAsync(id, request, CanViewFinance, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return Ok(result.Project);
    }

    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken) =>
        await projectService.ActivateAsync(id, cancellationToken) ? NoContent() : NotFound();

    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken) =>
        await projectService.DeactivateAsync(id, cancellationToken) ? NoContent() : NotFound();

    private ActionResult ValidationProblemFrom(IReadOnlyDictionary<string, string> errors)
    {
        foreach (var (field, message) in errors)
        {
            ModelState.AddModelError(field, message);
        }

        return ValidationProblem(ModelState);
    }
}