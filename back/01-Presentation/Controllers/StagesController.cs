using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;

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

    [HttpPost]
    public async Task<ActionResult<StageDto>> Create(SaveStageRequest request, CancellationToken cancellationToken)
    {
        var result = await stageService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Stage!.Id }, result.Stage);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<StageDto>> Update(long id, SaveStageRequest request, CancellationToken cancellationToken)
    {
        var result = await stageService.UpdateAsync(id, request, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return Ok(result.Stage);
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
