using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Presentation.Controllers;

[ApiController]
[Route("api/employment-regimes")]
[Authorize(Policy = "manage_employees")]
public class EmploymentRegimesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmploymentRegimesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<EmploymentRegimeDto>>> Get([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? search = null)
    {
        var query = _context.EmploymentRegimes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.Label.Contains(search) || (e.Description != null && e.Description.Contains(search)));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)size);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(e => new EmploymentRegimeDto
            {
                Id = e.Id,
                Label = e.Label,
                Unit = e.Unit,
                MonthlyHours = e.MonthlyHours,
                Description = e.Description,
                Active = e.Active
            })
            .ToListAsync();

        return Ok(new PagedResult<EmploymentRegimeDto>(items, page, size, total, totalPages));
    }

    [HttpPost]
    public async Task<ActionResult<EmploymentRegimeDto>> Create([FromBody] CreateEmploymentRegimeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Label))
            return BadRequest(new ProblemDetails { Detail = "Label is required." });

        if (string.IsNullOrWhiteSpace(request.Unit))
            return BadRequest(new ProblemDetails { Detail = "Unit is required." });

        var regime = new EmploymentRegime
        {
            Label = request.Label,
            Unit = request.Unit,
            MonthlyHours = request.MonthlyHours,
            Description = request.Description,
            Active = true
        };

        _context.EmploymentRegimes.Add(regime);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = regime.Id }, new EmploymentRegimeDto 
        { 
            Id = regime.Id, Label = regime.Label, Unit = regime.Unit, 
            MonthlyHours = regime.MonthlyHours, Description = regime.Description, Active = regime.Active 
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateEmploymentRegimeRequest request)
    {
        var regime = await _context.EmploymentRegimes.FindAsync(id);
        if (regime == null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Label))
            return BadRequest(new ProblemDetails { Detail = "Label is required." });

        if (string.IsNullOrWhiteSpace(request.Unit))
            return BadRequest(new ProblemDetails { Detail = "Unit is required." });

        regime.Label = request.Label;
        regime.Unit = request.Unit;
        regime.MonthlyHours = request.MonthlyHours;
        regime.Description = request.Description;
        regime.Active = request.Active;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var regime = await _context.EmploymentRegimes.FindAsync(id);
        if (regime == null) return NotFound();

        regime.Active = false; 
        await _context.SaveChangesAsync();
        return NoContent();
    }
}