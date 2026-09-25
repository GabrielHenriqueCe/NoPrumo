using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<DepartmentDto>>> Get([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string? search = null)
    {
        var query = _context.Departments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(d => d.Name.Contains(search));
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)size);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Active = d.Active
            })
            .ToListAsync();

        return Ok(new PagedResult<DepartmentDto>
        {
            Items = items,
            Page = page,
            Size = size,
            Total = total,
            TotalPages = totalPages
        });
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ProblemDetails { Detail = "Name is required." });

        var department = new Department
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Active = true
        };

        _context.Departments.Add(department);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = department.Id }, new DepartmentDto { Id = department.Id, Name = department.Name, Active = department.Active });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentRequest request)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new ProblemDetails { Detail = "Name is required." });

        department.Name = request.Name;
        department.Active = request.Active;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return NotFound();

        // Soft delete conforme a regra do guia
        department.Active = false; 
        
        await _context.SaveChangesAsync();
        return NoContent();
    }
}