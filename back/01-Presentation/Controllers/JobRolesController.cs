using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Presentation.Controllers;

[ApiController]
[Route("api/job-roles")] // <-- Rota fixa com hífen
[Authorize(Policy = "manage_employees")]
public class JobRolesController : ControllerBase
{
    private readonly AppDbContext _context;

    public JobRolesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string search = "")
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);
        var query = _context.JobRoles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(j => j.Name.Contains(search));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)size);

        var items = await query
            .OrderBy(jobRole => jobRole.Name)
            .ThenBy(jobRole => jobRole.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(j => new JobRoleDto
            {
                Id = j.Id,
                Name = j.Name,
                DepartmentId = j.DepartmentId,
                Active = j.Active
            })
            .ToListAsync();

        return Ok(new PagedResult<JobRoleDto>(items, page, size, total, totalPages));
    }

    [HttpPost]
    public async Task<ActionResult<JobRoleDto>> Create([FromBody] CreateJobRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ProblemDetails { Detail = "Name is required." });

        // Valida se o Setor existe
        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == request.DepartmentId);
        if (!departmentExists)
            return BadRequest(new ProblemDetails { Detail = "Invalid DepartmentId." });

        if (await _context.JobRoles.AnyAsync(jobRole => jobRole.Name == request.Name && jobRole.DepartmentId == request.DepartmentId))
        {
            ModelState.AddModelError("name", "Name already exists in this department.");
            return ValidationProblem(ModelState);
        }

        var jobRole = new JobRole
        {
            Name = request.Name,
            DepartmentId = request.DepartmentId,
            Active = true
        };

        _context.JobRoles.Add(jobRole);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = jobRole.Id }, new JobRoleDto { Id = jobRole.Id, Name = jobRole.Name, DepartmentId = jobRole.DepartmentId, Active = jobRole.Active });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateJobRoleRequest request)
    {
        var jobRole = await _context.JobRoles.FindAsync(id);
        if (jobRole == null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ProblemDetails { Detail = "Name is required." });

        if (!await _context.Departments.AnyAsync(department => department.Id == request.DepartmentId))
        {
            ModelState.AddModelError("departmentId", "Department not found.");
            return ValidationProblem(ModelState);
        }

        if (await _context.JobRoles.AnyAsync(jobRole => jobRole.Name == request.Name && jobRole.DepartmentId == request.DepartmentId && jobRole.Id != id))
        {
            ModelState.AddModelError("name", "Name already exists in this department.");
            return ValidationProblem(ModelState);
        }

        jobRole.Name = request.Name;
        jobRole.DepartmentId = request.DepartmentId;
        jobRole.Active = request.Active;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var jobRole = await _context.JobRoles.FindAsync(id);
        if (jobRole == null) return NotFound();

        jobRole.Active = false; 
        await _context.SaveChangesAsync();
        return NoContent();
    }
}