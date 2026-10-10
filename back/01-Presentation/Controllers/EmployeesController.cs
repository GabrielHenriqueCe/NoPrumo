using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;
using NoPrumo.Application.Services;

namespace NoPrumo.Presentation.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize(Policy = "manage_employees")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly DocumentProcessor _documentProcessor;

    public EmployeesController(AppDbContext context, DocumentProcessor documentProcessor)
    {
        _context = context;
        _documentProcessor = documentProcessor;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string search = "")
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);

        var query = _context.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.Name.Contains(search) || e.RegistrationNumber == search);
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)size);

        // Fazemos o JOIN manual para trazer os nomes (caso não existam as Navigation Properties configuradas)
        var employees = await query
            .OrderBy(employee => employee.Name)
            .ThenBy(employee => employee.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var jobRoleIds = employees.Where(e => e.JobRoleId.HasValue).Select(e => e.JobRoleId!.Value).Distinct();
        var regimeIds = employees.Where(e => e.EmploymentRegimeId.HasValue).Select(e => e.EmploymentRegimeId!.Value).Distinct();

        var jobRoles = await _context.JobRoles.Where(j => jobRoleIds.Contains(j.Id)).ToDictionaryAsync(j => j.Id, j => j.Name);
        var regimes = await _context.EmploymentRegimes.Where(r => regimeIds.Contains(r.Id)).ToDictionaryAsync(r => r.Id, r => r.Label);

        // Verifica a permissão financeira
        bool canViewFinance = User.HasClaim("permission", "view_finance");

        var items = employees.Select(e => 
        {
            var roleName = e.JobRoleId.HasValue && jobRoles.ContainsKey(e.JobRoleId.Value) ? jobRoles[e.JobRoleId.Value] : null;
            var regimeLabel = e.EmploymentRegimeId.HasValue && regimes.ContainsKey(e.EmploymentRegimeId.Value) ? regimes[e.EmploymentRegimeId.Value] : null;

            if (canViewFinance)
            {
                return (object)new EmployeeFinancialDto
                {
                    Id = e.Id, RegistrationNumber = e.RegistrationNumber, Name = e.Name,
                    JobRoleId = e.JobRoleId, JobRoleName = roleName,
                    EmploymentRegimeId = e.EmploymentRegimeId, EmploymentRegimeLabel = regimeLabel,
                    HireDate = e.HireDate, Phone = e.Phone, DocumentMasked = e.DocumentMasked,
                    Active = e.Active ?? false, PayRate = e.PayRate, AdditionalPercentage = e.AdditionalPercentage
                };
            }

            return (object)new EmployeeDto
            {
                Id = e.Id, RegistrationNumber = e.RegistrationNumber, Name = e.Name,
                JobRoleId = e.JobRoleId, JobRoleName = roleName,
                EmploymentRegimeId = e.EmploymentRegimeId, EmploymentRegimeLabel = regimeLabel,
                HireDate = e.HireDate, Phone = e.Phone, DocumentMasked = e.DocumentMasked,
                Active = e.Active ?? false
            };
        }).ToList();

        return Ok(new { items, page, size, total, totalPages });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "The Name field is required.");
            return ValidationProblem(ModelState);
        }

        if (!string.IsNullOrWhiteSpace(request.Document) && !DocumentProcessor.IsValid(request.Document))
        {
            ModelState.AddModelError("document", "Invalid document format.");
            return ValidationProblem(ModelState);
        }

        var employee = new Employee
        {
            Name = request.Name,
            RegistrationNumber = request.RegistrationNumber,
            JobRoleId = request.JobRoleId,
            EmploymentRegimeId = request.EmploymentRegimeId,
            HireDate = request.HireDate,
            Phone = request.Phone,
            Active = true
        };

        if (User.HasClaim("permission", "view_finance"))
        {
            employee.PayRate = request.PayRate;
            employee.AdditionalPercentage = request.AdditionalPercentage;
        }

        if (!string.IsNullOrWhiteSpace(request.Document))
        {
            var processedDoc = _documentProcessor.Process(request.Document);
            employee.DocumentMasked = processedDoc.Masked;
            employee.DocumentHash = processedDoc.Hash;
            employee.DocumentEncrypted = processedDoc.Encrypted;
        }

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        bool canViewFinance = User.HasClaim("permission", "view_finance");

        if (canViewFinance)
        {
            var financialDto = new EmployeeFinancialDto
            {
                Id = employee.Id,
                RegistrationNumber = employee.RegistrationNumber,
                Name = employee.Name,
                JobRoleId = employee.JobRoleId,
                EmploymentRegimeId = employee.EmploymentRegimeId,
                HireDate = employee.HireDate,
                Phone = employee.Phone,
                DocumentMasked = employee.DocumentMasked,
                Active = employee.Active ?? false,
                PayRate = employee.PayRate,
                AdditionalPercentage = employee.AdditionalPercentage
            };
            return CreatedAtAction(nameof(Get), new { id = employee.Id }, financialDto);
        }

        var dto = new EmployeeDto
        {
            Id = employee.Id,
            RegistrationNumber = employee.RegistrationNumber,
            Name = employee.Name,
            JobRoleId = employee.JobRoleId,
            EmploymentRegimeId = employee.EmploymentRegimeId,
            HireDate = employee.HireDate,
            Phone = employee.Phone,
            DocumentMasked = employee.DocumentMasked,
            Active = employee.Active ?? false
        };
        return CreatedAtAction(nameof(Get), new { id = employee.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "The Name field is required.");
            return ValidationProblem(ModelState);
        }

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        employee.Name = request.Name;
        employee.RegistrationNumber = request.RegistrationNumber;
        employee.JobRoleId = request.JobRoleId;
        employee.EmploymentRegimeId = request.EmploymentRegimeId;
        employee.HireDate = request.HireDate;
        employee.Phone = request.Phone;
        employee.Active = request.Active;

        // Só aplica as alterações financeiras se o utilizador tiver a claim
        if (User.HasClaim("permission", "view_finance"))
        {
            employee.PayRate = request.PayRate;
            employee.AdditionalPercentage = request.AdditionalPercentage;
        }

        if (!string.IsNullOrWhiteSpace(request.Document))
        {
            if (!DocumentProcessor.IsValid(request.Document))
            {
                ModelState.AddModelError("document", "Invalid document format.");
                return ValidationProblem(ModelState);
            }

            var processedDoc = _documentProcessor.Process(request.Document);
            employee.DocumentMasked = processedDoc.Masked;
            employee.DocumentHash = processedDoc.Hash;
            employee.DocumentEncrypted = processedDoc.Encrypted;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        employee.Active = false;
        employee.DeletedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }
}