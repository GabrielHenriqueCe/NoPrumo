using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Requests;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/departments")]
[Authorize(Policy = "manage_employees")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext appDbContext;

    public DepartmentsController(AppDbContext context)
    {
        appDbContext = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string search = "")
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);

        var query = appDbContext.Department.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(d => d.Name.Contains(search));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)size);

        var items = await query
            .OrderBy(department => department.Name)
            .ThenBy(department => department.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Active = d.Active
            })
            .ToListAsync();

        return Ok(new PagedResult<DepartmentDto>(items, page, size, total, totalPages));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var department = await appDbContext.Department.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (department == null) return NotFound();
        return Ok(new DepartmentDto { Id = department.Id, Name = department.Name, Active = department.Active });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }
        else if (await appDbContext.Department.AnyAsync(department => department.Name == request.Name))
        {
            ModelState.AddModelError("name", "Name already exists.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var department = new Department
        {
            Name = request.Name,
            Active = true
        };

        appDbContext.Department.Add(department);
        await appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = department.Id }, new DepartmentDto { Id = department.Id, Name = department.Name, Active = department.Active });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDepartmentRequest request)
    {
        var department = await appDbContext.Department.FindAsync(id);
        if (department == null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }
        else if (await appDbContext.Department.AnyAsync(department => department.Name == request.Name && department.Id != id))
        {
            ModelState.AddModelError("name", "Name already exists.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        department.Name = request.Name;

        await appDbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        var department = await appDbContext.Department.FindAsync(id);
        if (department == null) return NotFound();

        department.Active = true;
        await appDbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var department = await appDbContext.Department.FindAsync(id);
        if (department == null) return NotFound();

        department.Active = false;
        await appDbContext.SaveChangesAsync();
        return NoContent();
    }
}