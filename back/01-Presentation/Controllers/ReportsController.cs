using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "view_finance")]
public sealed class ReportsController(IProjectFinanceService projectFinanceService) : ControllerBase
{
    [HttpGet("project-closing")]
    public async Task<ActionResult<IReadOnlyList<ProjectClosingDto>>> ProjectClosing(CancellationToken cancellationToken) =>
        Ok(await projectFinanceService.ListClosingAsync(cancellationToken));
}
