using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_employees")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string search = "")
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);

        var query = _context.Departments.AsQueryable();

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

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ProblemDetails { Detail = "Name is required." });

        if (await _context.Departments.AnyAsync(department => department.Name == request.Name))
        {
            ModelState.AddModelError("name", "Name already exists.");
            return ValidationProblem(ModelState);
        }

        var department = new Department
        {
            Name = request.Name,
            Active = true
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = department.Id }, new DepartmentDto { Id = department.Id, Name = department.Name, Active = department.Active });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDepartmentRequest request)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ProblemDetails { Detail = "Name is required." });

        if (await _context.Departments.AnyAsync(department => department.Name == request.Name && department.Id != id))
        {
            ModelState.AddModelError("name", "Name already exists.");
            return ValidationProblem(ModelState);
        }

        department.Name = request.Name;
        department.Active = request.Active;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        department.Active = false; 
        
        await _context.SaveChangesAsync();
        return NoContent();
    }
}