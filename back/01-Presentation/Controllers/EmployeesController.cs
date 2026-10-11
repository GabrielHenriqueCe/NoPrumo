using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Requests;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;
using NoPrumo.Application.Services;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize(Policy = "manage_employees")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext appDbContext;
    private readonly DocumentProcessor _documentProcessor;

    public EmployeesController(AppDbContext context, DocumentProcessor documentProcessor)
    {
        appDbContext = context;
        _documentProcessor = documentProcessor;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string search = "")
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, 100);

        var query = appDbContext.Employee.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.Name.Contains(search) || e.RegistrationNumber == search);
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)size);

        var employees = await query
            .Include(e => e.JobRole)
            .Include(e => e.EmploymentRegime)
            .OrderBy(e => e.Name)
            .ThenBy(e => e.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        bool canViewFinance = User.HasClaim("permission", "view_finance");

        var items = employees.Select<Employee, object>(e =>
        {
            if (canViewFinance)
            {
                return new EmployeeFinancialDto
                {
                    Id = e.Id,
                    RegistrationNumber = e.RegistrationNumber,
                    Name = e.Name,
                    JobRoleId = e.JobRoleId,
                    JobRoleName = e.JobRole?.Name,
                    EmploymentRegimeId = e.EmploymentRegimeId,
                    EmploymentRegimeLabel = e.EmploymentRegime?.Label,
                    HireDate = e.HireDate,
                    Phone = e.Phone,
                    DocumentMasked = e.DocumentMasked,
                    Active = e.Active ?? false,
                    PayRate = e.PayRate,
                    AdditionalPercentage = e.AdditionalPercentage
                };
            }

            return new EmployeeDto
            {
                Id = e.Id,
                RegistrationNumber = e.RegistrationNumber,
                Name = e.Name,
                JobRoleId = e.JobRoleId,
                JobRoleName = e.JobRole?.Name,
                EmploymentRegimeId = e.EmploymentRegimeId,
                EmploymentRegimeLabel = e.EmploymentRegime?.Label,
                HireDate = e.HireDate,
                Phone = e.Phone,
                DocumentMasked = e.DocumentMasked,
                Active = e.Active ?? false
            };
        }).ToList();

        return Ok(new PagedResult<object>(items, page, size, total, totalPages));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var employee = await appDbContext.Employee
            .AsNoTracking()
            .Include(e => e.JobRole)
            .Include(e => e.EmploymentRegime)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null) return NotFound();

        bool canViewFinance = User.HasClaim("permission", "view_finance");

        if (canViewFinance)
        {
            return Ok(new EmployeeFinancialDto
            {
                Id = employee.Id,
                RegistrationNumber = employee.RegistrationNumber,
                Name = employee.Name,
                JobRoleId = employee.JobRoleId,
                JobRoleName = employee.JobRole?.Name,
                EmploymentRegimeId = employee.EmploymentRegimeId,
                EmploymentRegimeLabel = employee.EmploymentRegime?.Label,
                HireDate = employee.HireDate,
                Phone = employee.Phone,
                DocumentMasked = employee.DocumentMasked,
                Active = employee.Active ?? false,
                PayRate = employee.PayRate,
                AdditionalPercentage = employee.AdditionalPercentage
            });
        }

        return Ok(new EmployeeDto
        {
            Id = employee.Id,
            RegistrationNumber = employee.RegistrationNumber,
            Name = employee.Name,
            JobRoleId = employee.JobRoleId,
            JobRoleName = employee.JobRole?.Name,
            EmploymentRegimeId = employee.EmploymentRegimeId,
            EmploymentRegimeLabel = employee.EmploymentRegime?.Label,
            HireDate = employee.HireDate,
            Phone = employee.Phone,
            DocumentMasked = employee.DocumentMasked,
            Active = employee.Active ?? false
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "The Name field is required.");
        }
        else if (await appDbContext.Employee.AnyAsync(e => e.Name == request.Name))
        {
            ModelState.AddModelError("name", "This Name is already registered to another employee.");
        }

        if (string.IsNullOrWhiteSpace(request.RegistrationNumber))
        {
            ModelState.AddModelError("registrationNumber", "The Registration Number field is required.");
        }
        else if (await appDbContext.Employee.AnyAsync(e => e.RegistrationNumber == request.RegistrationNumber))
        {
            ModelState.AddModelError("registrationNumber", "This Registration Number already exists.");
        }

        if (!string.IsNullOrWhiteSpace(request.Document))
        {
            if (!DocumentProcessor.IsValid(request.Document))
            {
                ModelState.AddModelError("document", "Invalid document format.");
            }
            else
            {
                var processedDoc = _documentProcessor.Process(request.Document);
                if (await appDbContext.Employee.AnyAsync(e => e.DocumentHash == processedDoc.Hash))
                {
                    ModelState.AddModelError("document", "This document is already registered to another employee.");
                }
            }
        }

        if (request.JobRoleId.HasValue)
        {
            var role = await appDbContext.JobRole.FindAsync(request.JobRoleId);
            if (role == null || !role.Active)
            {
                ModelState.AddModelError("jobRoleId", "The selected Job Role is invalid or currently inactive.");
            }
        }

        if (request.EmploymentRegimeId.HasValue)
        {
            var regime = await appDbContext.EmploymentRegime.FindAsync(request.EmploymentRegimeId);
            if (regime == null || !regime.Active)
            {
                ModelState.AddModelError("employmentRegimeId", "The selected Employment Regime is invalid or currently inactive.");
            }
        }

        if (!ModelState.IsValid)
        {
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

        appDbContext.Employee.Add(employee);
        await appDbContext.SaveChangesAsync();

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
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, financialDto);
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
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateEmployeeRequest request)
    {
        var employee = await appDbContext.Employee.FindAsync(id);
        if (employee == null) return NotFound();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "The Name field is required.");
        }
        else if (await appDbContext.Employee.AnyAsync(e => e.Name == request.Name && e.Id != id))
        {
            ModelState.AddModelError("name", "An Employee with this Name already exists.");
        }

        if (string.IsNullOrWhiteSpace(request.RegistrationNumber))
        {
            ModelState.AddModelError("registrationNumber", "The Registration Number field is required.");
        }
        else if (await appDbContext.Employee.AnyAsync(e => e.RegistrationNumber == request.RegistrationNumber && e.Id != id))
        {
            ModelState.AddModelError("registrationNumber", "This Registration Number already exists.");
        }

        if (!string.IsNullOrWhiteSpace(request.Document))
        {
            if (!DocumentProcessor.IsValid(request.Document))
            {
                ModelState.AddModelError("document", "Invalid document format.");
            }
            else
            {
                var processedDoc = _documentProcessor.Process(request.Document);
                if (await appDbContext.Employee.AnyAsync(e => e.DocumentHash == processedDoc.Hash && e.Id != id))
                {
                    ModelState.AddModelError("document", "This document is already registered to another employee.");
                }
            }
        }

        if (request.JobRoleId.HasValue && request.JobRoleId != employee.JobRoleId)
        {
            var role = await appDbContext.JobRole.FindAsync(request.JobRoleId);
            if (role == null || !role.Active)
            {
                ModelState.AddModelError("jobRoleId", "The selected Job Role is invalid or currently inactive.");
            }
        }

        if (request.EmploymentRegimeId.HasValue && request.EmploymentRegimeId != employee.EmploymentRegimeId)
        {
            var regime = await appDbContext.EmploymentRegime.FindAsync(request.EmploymentRegimeId);
            if (regime == null || !regime.Active)
            {
                ModelState.AddModelError("employmentRegimeId", "The selected Employment Regime is invalid or currently inactive.");
            }
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        employee.Name = request.Name;
        employee.RegistrationNumber = request.RegistrationNumber;
        employee.JobRoleId = request.JobRoleId;
        employee.EmploymentRegimeId = request.EmploymentRegimeId;
        employee.HireDate = request.HireDate;
        employee.Phone = request.Phone;

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

        await appDbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        var employee = await appDbContext.Employee.FindAsync(id);
        if (employee == null) return NotFound();

        employee.Active = true;
        employee.DeletedAt = null;
        await appDbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var employee = await appDbContext.Employee.FindAsync(id);
        if (employee == null) return NotFound();

        employee.Active = false;
        employee.DeletedAt = DateTime.UtcNow;
        await appDbContext.SaveChangesAsync();
        return NoContent();
    }
}