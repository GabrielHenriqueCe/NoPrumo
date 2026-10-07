using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Extensions;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// A ficha de treinamento: cada linha é um certificado de um funcionário
/// (NR-35, ASO...). Não há desativar nem apagar — a tabela não tem
/// Active/DeletedAt, e certificado é histórico.
///
/// ExpiryDate é sempre calculado pela API (TrainingType.ExpiryFor). O front
/// nunca manda vencimento; só mostra o que voltou.
/// </summary>
[ApiController]
[Route("api/[controller]")] // vira /api/employeetrainings
[Authorize(Policy = "manage_safety")]
public sealed class EmployeeTrainingsController(AppDbContext db, TimeProvider timeProvider) : ControllerBase
{
    private const int MaxPageSize = 100;
    private const int MaxWorkloadHours = 1000;
    private const int MaxInstructorLength = 160;

    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeTrainingDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null,
        [FromQuery] long? employeeId = null,
        [FromQuery] long? trainingTypeId = null)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var query = db.EmployeeTraining
            .AsNoTracking()
            .Include(t => t.Employee)
            .Include(t => t.TrainingType)
            .AsQueryable();

        if (employeeId is not null)
        {
            query = query.Where(t => t.EmployeeId == employeeId);
        }

        if (trainingTypeId is not null)
        {
            query = query.Where(t => t.TrainingTypeId == trainingTypeId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(t =>
                t.Employee.Name.Contains(term) ||
                (t.Employee.RegistrationNumber != null && t.Employee.RegistrationNumber.Contains(term)) ||
                t.TrainingType.Code.Contains(term) ||
                t.TrainingType.Name.Contains(term) ||
                (t.Instructor != null && t.Instructor.Contains(term)));
        }

        var total = await query.CountAsync();

        // O mais recente primeiro: é o certificado que vale hoje.
        var items = await query
            .OrderByDescending(t => t.IssueDate)
            .ThenBy(t => t.Employee.Name)
            .ThenBy(t => t.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        var today = timeProvider.Today();

        return Ok(new PagedResult<EmployeeTrainingDto>(
            items.Select(training => ToDto(training, today)).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<EmployeeTrainingDto>> GetById(long id)
    {
        var training = await db.EmployeeTraining
            .AsNoTracking()
            .Include(t => t.Employee)
            .Include(t => t.TrainingType)
            .SingleOrDefaultAsync(t => t.Id == id);

        if (training is null) return NotFound();

        return Ok(ToDto(training, timeProvider.Today()));
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeTrainingDto>> Create(SaveEmployeeTrainingRequest request)
    {
        var modality = request.Modality;
        var instructor = string.IsNullOrWhiteSpace(request.Instructor) ? null : request.Instructor.Trim();

        var (employee, type) = await ValidateAsync(
            request.EmployeeId,
            request.TrainingTypeId,
            request.IssueDate,
            request.WorkloadHours,
            modality,
            instructor,
            current: null);

        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var issueDate = request.IssueDate!.Value;

        var training = new EmployeeTraining
        {
            EmployeeId = employee!.Id,
            Employee = employee,
            TrainingTypeId = type!.Id,
            TrainingType = type,
            IssueDate = issueDate,
            ExpiryDate = type.ExpiryFor(issueDate),
            WorkloadHours = request.WorkloadHours,
            Modality = modality,
            Instructor = instructor,
        };

        db.EmployeeTraining.Add(training);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, ToDto(training, timeProvider.Today()));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<EmployeeTrainingDto>> Update(long id, SaveEmployeeTrainingRequest request)
    {
        var training = await db.EmployeeTraining
            .Include(t => t.Employee)
            .Include(t => t.TrainingType)
            .SingleOrDefaultAsync(t => t.Id == id);

        if (training is null) return NotFound();

        var modality = request.Modality;
        var instructor = string.IsNullOrWhiteSpace(request.Instructor) ? null : request.Instructor.Trim();

        var (employee, type) = await ValidateAsync(
            request.EmployeeId,
            request.TrainingTypeId,
            request.IssueDate,
            request.WorkloadHours,
            modality,
            instructor,
            current: training);

        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var issueDate = request.IssueDate!.Value;

        // Só recalcula se mudou o que entra na conta. Corrigir o nome do
        // instrutor não pode mexer num vencimento calculado com a validade que
        // valia quando o certificado foi registrado.
        if (training.IssueDate != issueDate || training.TrainingTypeId != type!.Id)
        {
            training.ExpiryDate = type!.ExpiryFor(issueDate);
        }

        training.EmployeeId = employee!.Id;
        training.Employee = employee;
        training.TrainingTypeId = type!.Id;
        training.TrainingType = type;
        training.IssueDate = issueDate;
        training.WorkloadHours = request.WorkloadHours;
        training.Modality = modality;
        training.Instructor = instructor;

        await db.SaveChangesAsync();

        return Ok(ToDto(training, timeProvider.Today()));
    }

    // Valida os campos e devolve o funcionário e o tipo (ou null, já com os
    // erros no ModelState). Cada erro leva o nome do campo da tela.
    private async Task<(Employee? Employee, TrainingType? Type)> ValidateAsync(
        long? employeeId,
        long? trainingTypeId,
        DateOnly? issueDate,
        int? workloadHours,
        TrainingModality? modality,
        string? instructor,
        EmployeeTraining? current)
    {
        // Rastreados de propósito: o treinamento passa a apontar para eles, e
        // um objeto não rastreado faria o EF tentar inseri-lo de novo.
        var employee = employeeId is null
            ? null
            : await db.Employees.SingleOrDefaultAsync(e => e.Id == employeeId);

        if (employee is null)
        {
            ModelState.AddModelError("employeeId", "Pick an employee.");
        }
        else if (employee.Active != true && employee.Id != current?.EmployeeId)
        {
            // Funcionário desligado não recebe certificado novo; o que ele já
            // tinha continua na ficha e pode ser corrigido.
            ModelState.AddModelError("employeeId", "This employee is inactive.");
        }

        var type = trainingTypeId is null
            ? null
            : await db.TrainingType.SingleOrDefaultAsync(t => t.Id == trainingTypeId);

        if (type is null)
        {
            ModelState.AddModelError("trainingTypeId", "Pick a training type.");
        }

        if (issueDate is null)
        {
            ModelState.AddModelError("issueDate", "Issue date is required.");
        }
        else if (issueDate > timeProvider.Today())
        {
            ModelState.AddModelError("issueDate", "The issue date cannot be in the future.");
        }

        if (workloadHours is < 1 or > MaxWorkloadHours)
        {
            ModelState.AddModelError("workloadHours", $"Use 1 to {MaxWorkloadHours} hours, or leave it blank.");
        }

        if (instructor is { Length: > MaxInstructorLength })
        {
            ModelState.AddModelError("instructor", $"Use at most {MaxInstructorLength} characters.");
        }

        // As regras que vêm do tipo (carga mínima, presencial) valem para o que
        // se lança agora. Num certificado antigo, só voltam a valer se mexerem
        // no tipo, na carga ou na modalidade — senão corrigir um erro de
        // digitação esbarraria numa regra que nem existia quando ele foi emitido.
        var typeRulesApply = current is null
            || current.TrainingTypeId != trainingTypeId
            || current.WorkloadHours != workloadHours
            || current.Modality != modality;

        if (type is not null && typeRulesApply)
        {
            if (workloadHours is { } hours && type.MinWorkloadHours is { } min && hours < min)
            {
                ModelState.AddModelError("workloadHours", $"{type.Code} requires at least {min} hours.");
            }

            if (type.RequiresInPerson && modality != TrainingModality.InPerson)
            {
                ModelState.AddModelError("modality", $"{type.Code} must be taken in person.");
            }
        }

        // O mesmo certificado lançado duas vezes (duplo clique, ou a mesma folha
        // digitada por duas pessoas) apareceria como dois treinamentos.
        var ignoreId = current?.Id ?? 0;

        if (ModelState.IsValid && await db.EmployeeTraining.AnyAsync(t =>
                t.Id != ignoreId &&
                t.EmployeeId == employee!.Id &&
                t.TrainingTypeId == type!.Id &&
                t.IssueDate == issueDate!.Value))
        {
            ModelState.AddModelError("issueDate", "This employee already has this training recorded on this date.");
        }

        return (employee, type);
    }

    // Exige Employee e TrainingType carregados.
    private static EmployeeTrainingDto ToDto(EmployeeTraining training, DateOnly today) => new()
    {
        Id = training.Id,
        EmployeeId = training.EmployeeId,
        EmployeeName = training.Employee.Name,
        RegistrationNumber = training.Employee.RegistrationNumber,
        TrainingTypeId = training.TrainingTypeId,
        TrainingTypeCode = training.TrainingType.Code,
        TrainingTypeName = training.TrainingType.Name,
        IssueDate = training.IssueDate,
        ExpiryDate = training.ExpiryDate,
        Status = training.StatusOn(today),
        WorkloadHours = training.WorkloadHours,
        Modality = training.Modality,
        Instructor = training.Instructor,
    };
}
