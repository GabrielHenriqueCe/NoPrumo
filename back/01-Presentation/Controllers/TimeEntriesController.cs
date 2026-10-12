using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Requests;
using NoPrumo.Application.Interfaces;

namespace NoPrumo.Presentation.Controllers;

[ApiController]
[Route("api/time-entries")]
[Authorize(Policy = "manage_schedule")]
public class TimeEntriesController(ITimeEntryService timeEntryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] long projectId,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        CancellationToken cancellationToken = default)
    {
        if (projectId <= 0)
            return BadRequest(new { Message = "projectId é obrigatório." });

        if (from > to)
            return BadRequest(new { Message = "A data 'from' não pode ser superior a 'to'." });

        bool canViewFinance = User.HasClaim("permission", "view_finance");

        var result = await timeEntryService.ListAsync(page, size, projectId, from, to, canViewFinance, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimeEntryRequest request, CancellationToken cancellationToken)
    {
        if (request.ProjectId <= 0 || request.EmployeeId <= 0)
            return BadRequest(new { Message = "ProjectId e EmployeeId são obrigatórios." });

        if (request.ClockOut.HasValue && request.ClockOut < request.ClockIn)
            return BadRequest(new { Message = "A hora de saída (ClockOut) não pode ser inferior à entrada (ClockIn)." });

        try
        {
            // Extrai o ID do utilizador do JWT de forma segura e nativa
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;
            long.TryParse(userIdClaim, out long userId);

            bool canViewFinance = User.HasClaim("permission", "view_finance");

            var dto = await timeEntryService.SaveAsync(request, userId, canViewFinance, cancellationToken);

            return CreatedAtAction(nameof(List), new { projectId = request.ProjectId, from = request.Date, to = request.Date }, dto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}