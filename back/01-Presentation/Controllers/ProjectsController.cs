using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Projects;
using NoPrumo.Application.Interfaces;

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
}