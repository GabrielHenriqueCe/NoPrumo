using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PortalController(IProjectLinkService projectLinkService) : ControllerBase
{
    [HttpGet("{token}")]
    [AllowAnonymous]
    public async Task<ActionResult<PortalProjectDto>> GetProjectByToken(
        string token,
        CancellationToken cancellationToken)
    {
        var project = await projectLinkService.GetPortalProjectAsync(token, cancellationToken);
        if (project is null)
        {
            return NotFound(new { message = "Portal link is invalid, expired, or revoked." });
        }

        return Ok(project);
    }
}
