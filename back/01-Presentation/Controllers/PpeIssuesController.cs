using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;

namespace NoPrumo.Controllers;

// Ler: segurança, mestre e almoxarife (view_ppe). Entregar, assinar e dar baixa: manage_safety.
[ApiController]
[Route("api/ppe-issues")]
[Authorize(Policy = "view_ppe")]
public sealed class PpeIssuesController(IPpeIssueService ppeIssueService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<PpeIssueDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null,
        [FromQuery] long? projectId = null,
        [FromQuery] long? employeeId = null,
        CancellationToken cancellationToken = default) =>
        Ok(await ppeIssueService.ListAsync(page, size, search, projectId, employeeId, cancellationToken));

    [HttpGet("options")]
    [Authorize(Policy = "manage_safety")]
    public async Task<ActionResult<PpeIssueOptionsDto>> GetOptions(CancellationToken cancellationToken) =>
        Ok(await ppeIssueService.GetOptionsAsync(cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PpeIssueDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var issue = await ppeIssueService.GetByIdAsync(id, cancellationToken);
        return issue is null ? NotFound() : Ok(issue);
    }

    [HttpPost]
    [Authorize(Policy = "manage_safety")]
    public async Task<ActionResult<PpeIssueDto>> Create(SavePpeIssueRequest request, CancellationToken cancellationToken)
    {
        var result = await ppeIssueService.CreateAsync(request, CurrentUserId(), cancellationToken);
        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Issue!.Id }, result.Issue);
    }

    [HttpPost("{id:long}/sign")]
    [Authorize(Policy = "manage_safety")]
    public async Task<ActionResult<PpeIssueDto>> Sign(long id, SignPpeIssueRequest request, CancellationToken cancellationToken)
    {
        var result = await ppeIssueService.SignAsync(id, request, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return result.Succeeded ? Ok(result.Issue) : ValidationProblemFrom(result.Errors);
    }

    [HttpPatch("{id:long}/items/{itemId:long}/return")]
    [Authorize(Policy = "manage_safety")]
    public async Task<ActionResult<PpeIssueDto>> ReturnItem(long id, long itemId, ReturnPpeItemRequest request, CancellationToken cancellationToken)
    {
        var result = await ppeIssueService.ReturnItemAsync(id, itemId, request, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return result.Succeeded ? Ok(result.Issue) : ValidationProblemFrom(result.Errors);
    }

    [HttpPatch("{id:long}/items/{itemId:long}/lost")]
    [Authorize(Policy = "manage_safety")]
    public async Task<ActionResult<PpeIssueDto>> MarkItemLost(long id, long itemId, CancellationToken cancellationToken)
    {
        var result = await ppeIssueService.MarkItemLostAsync(id, itemId, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return result.Succeeded ? Ok(result.Issue) : ValidationProblemFrom(result.Errors);
    }

    private long? CurrentUserId() =>
        long.TryParse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value, out var userId) ? userId : null;

    private ActionResult ValidationProblemFrom(IReadOnlyDictionary<string, string> errors)
    {
        foreach (var (field, message) in errors)
        {
            ModelState.AddModelError(field, message);
        }

        return ValidationProblem(ModelState);
    }
}
