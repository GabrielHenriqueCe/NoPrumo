using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Requests;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;
using NoPrumo.Application.Services;

namespace NoPrumo.Presentation.Controllers;

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
            query = query.Where(employee => employee.Name.Contains(search) || employee.RegistrationNumber == search);
        }

        var total = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(total / (double)size);

        var employees = await query
            .Include(employee => employee.JobRole)
            .Include(employee => employee.EmploymentRegime)
            .OrderBy(employee => employee.Name)
            .ThenBy(employee => employee.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        bool canViewFinance = User.HasClaim("permission", "view_finance");

        var items = employees.Select<Employee, object>(employee =>
        {
            if (canViewFinance)
            {
                return new EmployeeFinancialDto
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
                };
            }

            return new EmployeeDto
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
            };
        }).ToList();

        return Ok(new PagedResult<object>(items, page, size, total, totalPages));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var employee = await appDbContext.Employee
            .AsNoTracking()
            .Include(employee => employee.JobRole)
            .Include(employee => employee.EmploymentRegime)
            // Equipe atual (EmployeeTeam sem EndDate)
            .Include(employee => employee.EmployeeTeams.Where(employeeTeam => employeeTeam.EndDate == null))
                .ThenInclude(employeeTeam => employeeTeam.Team)
                    .ThenInclude(team => team.TeamProjects)
                        .ThenInclude(teamProject => teamProject.Project)
                            .ThenInclude(project => project.Client)
            .Include(employee => employee.EmployeeTeams.Where(employeeTeam => employeeTeam.EndDate == null))
                .ThenInclude(employeeTeam => employeeTeam.Team)
                    .ThenInclude(team => team.TeamProjects)
                        .ThenInclude(teamProject => teamProject.Project)
                            .ThenInclude(project => project.Supervisor)
            // Treinamentos para a função LatestTrainings funcionar
            .Include(employee => employee.EmployeeTrainings)
                .ThenInclude(training => training.TrainingType)
            .FirstOrDefaultAsync(employee => employee.Id == id);

        if (employee == null) return NotFound();

        var currentEmployeeTeam = employee.EmployeeTeams.FirstOrDefault();
        var currentTeam = currentEmployeeTeam?.Team;

        var currentProjects = currentTeam?.TeamProjects
            .Select(teamProject => teamProject.Project)
            .Where(project => project != null)
            .Select(project => new ProjectDto
            {
                Id = project.Id,
                Code = project.Code,
                Name = project.Name,
                Description = project.Description,
                ClientId = project.ClientId,
                ClientName = project.Client?.Name,
                Cno = project.Cno,
                Street = project.Street,
                Number = project.Number,
                Complement = project.Complement,
                District = project.District,
                City = project.City,
                State = project.State,
                PostalCode = project.PostalCode,
                Status = project.Status,
                SupervisorId = project.SupervisorId,
                SupervisorName = project.Supervisor?.Name,
                TechnicalManager = project.TechnicalManager,
                CreaRt = project.CreaRt,
                StartDate = project.StartDate,
                ForecastDate = project.ForecastDate,
                CompletionDate = project.CompletionDate,
                Notes = project.Notes,
                Late = project.IsLate(today),
                Active = project.DeletedAt == null
            })
            .ToList() ?? new List<ProjectDto>();

        var latestTrainings = employee.LatestTrainings()
            .Select(training => new EmployeeTrainingDetailDto
            {
                Id = training.Id,
                TypeName = training.TrainingType?.Name ?? string.Empty,
                IssueDate = training.IssueDate,
                ExpiryDate = training.ExpiryDate,
                Status = training.StatusOn(today)
            })
            .ToList();

        bool canViewFinance = User.HasClaim("permission", "view_finance");

        if (canViewFinance)
        {
            return Ok(new EmployeeDetailFinanceDto
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
                AdditionalPercentage = employee.AdditionalPercentage,
                CurrentTeamName = currentTeam?.Name,
                TeamAllocationDate = currentEmployeeTeam?.StartDate,
                CurrentProjects = currentProjects,
                Trainings = latestTrainings
            });
        }

        return Ok(new EmployeeDetailDto
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
            CurrentTeamName = currentTeam?.Name,
            TeamAllocationDate = currentEmployeeTeam?.StartDate,
            CurrentProjects = currentProjects,
            Trainings = latestTrainings
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            ModelState.AddModelError("name", "The Name field is required.");
        }
        else if (await appDbContext.Employee.AnyAsync(employee => employee.Name == request.Name))
        {
            ModelState.AddModelError("name", "This Name is already registered to another employee.");
        }

        if (string.IsNullOrWhiteSpace(request.RegistrationNumber))
        {
            ModelState.AddModelError("registrationNumber", "The Registration Number field is required.");
        }
        else if (await appDbContext.Employee.AnyAsync(employee => employee.RegistrationNumber == request.RegistrationNumber))
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
                if (await appDbContext.Employee.AnyAsync(employee => employee.DocumentHash == processedDoc.Hash))
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
        else if (await appDbContext.Employee.AnyAsync(employeeCheck => employeeCheck.Name == request.Name && employeeCheck.Id != id))
        {
            ModelState.AddModelError("name", "An Employee with this Name already exists.");
        }

        if (string.IsNullOrWhiteSpace(request.RegistrationNumber))
        {
            ModelState.AddModelError("registrationNumber", "The Registration Number field is required.");
        }
        else if (await appDbContext.Employee.AnyAsync(employeeCheck => employeeCheck.RegistrationNumber == request.RegistrationNumber && employeeCheck.Id != id))
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
                if (await appDbContext.Employee.AnyAsync(employeeCheck => employeeCheck.DocumentHash == processedDoc.Hash && employeeCheck.Id != id))
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