using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Requests;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/job-roles")] // <-- Rota fixa com hífen
[Authorize(Policy = "manage_employees")]
public class JobRolesController : ControllerBase
{
    private readonly AppDbContext appDbContext;

    public JobRolesController(AppDbContext context)
    {
        appDbContext = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string search = "")
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);
        var query = appDbContext.JobRole.AsNoTracking();

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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var jobRole = await appDbContext.JobRole.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (jobRole == null) return NotFound();
        return Ok(new JobRoleDto { Id = jobRole.Id, Name = jobRole.Name, DepartmentId = jobRole.DepartmentId, Active = jobRole.Active });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }
        else if (await appDbContext.JobRole.AnyAsync(jobRole => jobRole.Name == request.Name))
        {
            ModelState.AddModelError("name", "A Job Role with this name already exists.");
        }

        if (request.DepartmentId <= 0)
        {
            ModelState.AddModelError("departmentId", "Department is required.");
        }
        else if (!await appDbContext.Department.AnyAsync(d => d.Id == request.DepartmentId))
        {
            ModelState.AddModelError("departmentId", "Invalid DepartmentId.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var jobRole = new JobRole
        {
            Name = request.Name,
            DepartmentId = request.DepartmentId,
            Active = true
        };

        appDbContext.JobRole.Add(jobRole);
        await appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = jobRole.Id }, new JobRoleDto { Id = jobRole.Id, Name = jobRole.Name, DepartmentId = jobRole.DepartmentId, Active = jobRole.Active });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateJobRoleRequest request)
    {
        var jobRole = await appDbContext.JobRole.FindAsync(id);
        if (jobRole == null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }
        else if (await appDbContext.JobRole.AnyAsync(jobRole => jobRole.Name == request.Name && jobRole.Id != id))
        {
            ModelState.AddModelError("name", "A Job Role with this name already exists.");
        }

        if (request.DepartmentId <= 0)
        {
            ModelState.AddModelError("departmentId", "Department is required.");
        }
        else if (request.DepartmentId != jobRole.DepartmentId && !await appDbContext.Department.AnyAsync(department => department.Id == request.DepartmentId))
        {
            ModelState.AddModelError("departmentId", "Invalid DepartmentId.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        jobRole.Name = request.Name;
        jobRole.DepartmentId = request.DepartmentId;

        await appDbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        var jobRole = await appDbContext.JobRole.FindAsync(id);
        if (jobRole == null) return NotFound();

        jobRole.Active = true;
        await appDbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var jobRole = await appDbContext.JobRole.FindAsync(id);
        if (jobRole == null) return NotFound();

        jobRole.Active = false;
        await appDbContext.SaveChangesAsync();
        return NoContent();
    }
}