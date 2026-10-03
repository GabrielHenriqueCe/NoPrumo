using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_projects")]
public sealed class StagesController(IStageService stageService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<StageDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null,
        [FromQuery] long? projectId = null,
        CancellationToken cancellationToken = default) =>
        Ok(await stageService.ListAsync(page, size, search, projectId, cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<StageDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var stage = await stageService.GetByIdAsync(id, cancellationToken);
        return stage is null ? NotFound() : Ok(stage);
    }
}
