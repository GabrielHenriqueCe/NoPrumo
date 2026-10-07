using System.Globalization;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Cadastro de equipes, com os membros dentro — não existe tela própria de
/// membro.
///
/// A equipe é desativada, nunca apagada. Team não tem coluna active: desativar
/// é preencher DeletedAt, o que também a tira do índice único de nome por
/// setor (pela coluna calculada active_key).
///
/// Membro sai com data, não sumindo. Encerrar a passagem preenche EndDate e
/// mantém a linha — é ela que responde quem estava em qual equipe, e quando.
/// </summary>
[ApiController]
[Route("api/[controller]")] // vira /api/teams
[Authorize(Policy = "manage_employees")]
public sealed class TeamsController(AppDbContext db) : ControllerBase
{
    private const int MaxPageSize = 100;
    private const int MaxNameLength = 120;

    // Uma projeção só para a lista, o detalhe e as respostas de criar e
    // editar. A contagem de membros vira subconsulta no SQL, sem trazer as
    // passagens para a memória.
    private static readonly Expression<Func<Team, TeamDto>> ToDto = t => new TeamDto(
        t.Id,
        t.Name,
        t.DepartmentId,
        t.Department.Name,
        t.DeletedAt == null,
        t.EmployeeTeams.Count(m => m.EndDate == null));

    [HttpGet]
    public async Task<ActionResult<PagedResult<TeamDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null,
        [FromQuery] long? departmentId = null)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var query = db.Team.AsNoTracking().AsQueryable();

        if (departmentId is not null)
        {
            query = query.Where(t => t.DepartmentId == departmentId);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t => t.Name.Contains(term) || t.Department.Name.Contains(term));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.Name)
            .ThenBy(t => t.Id)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(ToDto)
            .ToListAsync();

        return Ok(new PagedResult<TeamDto>(
            items,
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<TeamDto>> GetById(long id)
    {
        var team = await LoadDtoAsync(id);

        if (team is null) return NotFound();

        return Ok(team);
    }

    [HttpPost]
    public async Task<ActionResult<TeamDto>> Create(SaveTeamRequest request)
    {
        var name = request.Name?.Trim() ?? string.Empty;

        await ValidateAsync(name, request.DepartmentId, current: null);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var team = new Team
        {
            Name = name,
            DepartmentId = request.DepartmentId!.Value,
        };

        db.Team.Add(team);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, await LoadDtoAsync(team.Id));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<TeamDto>> Update(long id, SaveTeamRequest request)
    {
        var team = await db.Team.SingleOrDefaultAsync(t => t.Id == id);

        if (team is null) return NotFound();

        var name = request.Name?.Trim() ?? string.Empty;

        await ValidateAsync(name, request.DepartmentId, team);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        team.Name = name;
        team.DepartmentId = request.DepartmentId!.Value;

        await db.SaveChangesAsync();

        return Ok(await LoadDtoAsync(id));
    }

    [HttpPatch("{id:long}/activate")]
    public async Task<IActionResult> Activate(long id)
    {
        var team = await db.Team.SingleOrDefaultAsync(t => t.Id == id);

        if (team is null) return NotFound();
        if (team.DeletedAt is null) return NoContent();

        // Reativar devolve a equipe ao índice único de nome por setor. Se outra
        // equipe ativa já usa o nome, o banco recusaria — com erro 500.
        if (await db.Team.AnyAsync(t =>
                t.Id != id &&
                t.Name == team.Name &&
                t.DepartmentId == team.DepartmentId &&
                t.DeletedAt == null))
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: "Another active team in this department already has this name. Rename one of them first.");
        }

        team.DeletedAt = null;
        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id:long}/deactivate")]
    public async Task<IActionResult> Deactivate(long id)
    {
        var team = await db.Team.SingleOrDefaultAsync(t => t.Id == id);

        if (team is null) return NotFound();
        if (team.DeletedAt is not null) return NoContent();

        // Desativar com gente dentro deixaria passagens abertas numa equipe que
        // não existe mais. Cada saída tem a sua data, e só quem conhece a
        // equipe sabe qual é — por isso o encerramento fica explícito, membro
        // a membro, em vez de a API inventar a data de hoje para todos.
        var current = await db.EmployeeTeam.CountAsync(m => m.TeamId == id && m.EndDate == null);

        if (current > 0)
        {
            var members = current == 1 ? "1 current member" : $"{current} current members";

            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: $"This team still has {members}. End their stints before deactivating it.");
        }

        team.DeletedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{id:long}/members")]
    public async Task<ActionResult<IReadOnlyList<TeamMemberDto>>> ListMembers(long id)
    {
        if (!await db.Team.AnyAsync(t => t.Id == id)) return NotFound();

        // Quem está na equipe vem primeiro, em ordem alfabética; depois o
        // histórico, da saída mais recente para a mais antiga.
        var members = await db.EmployeeTeam
            .AsNoTracking()
            .Include(m => m.Employee)
            .Where(m => m.TeamId == id)
            .OrderBy(m => m.EndDate != null)
            .ThenByDescending(m => m.EndDate)
            .ThenBy(m => m.Employee.Name)
            .ToListAsync();

        return Ok(members.Select(TeamMemberDto.FromEntity).ToArray());
    }

    [HttpPost("{id:long}/members")]
    public async Task<ActionResult<TeamMemberDto>> AddMember(long id, AddTeamMemberRequest request)
    {
        var team = await db.Team.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);

        if (team is null) return NotFound();

        if (team.DeletedAt is not null)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                detail: "This team is inactive. Activate it before adding members.");
        }

        if (request.StartDate is null)
        {
            ModelState.AddModelError("startDate", "Start date is required.");
        }

        // Rastreado de propósito: a passagem nova aponta para ele, e um
        // funcionário não rastreado faria o EF tentar inseri-lo de novo.
        var employee = request.EmployeeId is null
            ? null
            : await db.Employees.SingleOrDefaultAsync(e => e.Id == request.EmployeeId);

        if (employee is null)
        {
            ModelState.AddModelError("employeeId", "Pick an employee.");
        }
        else if (employee.Active != true)
        {
            ModelState.AddModelError("employeeId", "This employee is inactive.");
        }
        else if (request.StartDate is { } startDate)
        {
            // Uma passagem nova não pode cruzar outra do mesmo funcionário na
            // mesma equipe: a aberta ainda não terminou, e uma fechada que
            // termina no dia da nova entrada (ou depois) se sobrepõe a ela.
            var endDates = await db.EmployeeTeam
                .AsNoTracking()
                .Where(m => m.TeamId == id && m.EmployeeId == employee.Id)
                .Select(m => m.EndDate)
                .ToListAsync();

            if (endDates.Any(endDate => endDate is null))
            {
                ModelState.AddModelError("employeeId", "This employee is already a member of this team.");
            }
            else if (endDates.Max() is { } lastEnd && lastEnd >= startDate)
            {
                ModelState.AddModelError(
                    "startDate",
                    $"This employee was in this team until {Format(lastEnd)}. The new stint has to start after that.");
            }
        }

        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var member = new EmployeeTeam
        {
            TeamId = id,
            EmployeeId = employee!.Id,
            Employee = employee,
            StartDate = request.StartDate!.Value,
        };

        db.EmployeeTeam.Add(member);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, TeamMemberDto.FromEntity(member));
    }

    [HttpPatch("{id:long}/members/{employeeId:long}/end")]
    public async Task<ActionResult<TeamMemberDto>> EndMember(long id, long employeeId, EndTeamMemberRequest request)
    {
        // Só a passagem aberta se encerra por aqui; as fechadas são histórico.
        var member = await db.EmployeeTeam
            .Include(m => m.Employee)
            .Where(m => m.TeamId == id && m.EmployeeId == employeeId && m.EndDate == null)
            .OrderByDescending(m => m.StartDate)
            .FirstOrDefaultAsync();

        if (member is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                detail: "This employee is not a current member of this team.");
        }

        if (request.EndDate is null)
        {
            ModelState.AddModelError("endDate", "End date is required.");
        }
        else if (request.EndDate < member.StartDate)
        {
            // Era o CHECK chk_funcionario_equipes_datas do banco antigo. A
            // migration de hoje não tem CHECK, então a regra mora aqui.
            ModelState.AddModelError(
                "endDate",
                $"The end date cannot be before the start date ({Format(member.StartDate)}).");
        }

        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        member.EndDate = request.EndDate;
        await db.SaveChangesAsync();

        return Ok(TeamMemberDto.FromEntity(member));
    }

    private Task<TeamDto?> LoadDtoAsync(long id) =>
        db.Team.AsNoTracking().Where(t => t.Id == id).Select(ToDto).SingleOrDefaultAsync();

    // Valida nome e setor. Cada erro leva o nome do campo, e é isso que faz a
    // mensagem aparecer embaixo do campo certo na tela.
    private async Task ValidateAsync(string name, long? departmentId, Team? current)
    {
        if (string.IsNullOrEmpty(name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }
        else if (name.Length > MaxNameLength)
        {
            ModelState.AddModelError("name", $"Use at most {MaxNameLength} characters.");
        }

        var department = departmentId is null
            ? null
            : await db.Department.AsNoTracking().SingleOrDefaultAsync(d => d.Id == departmentId);

        if (department is null)
        {
            ModelState.AddModelError("departmentId", "Pick a department.");
        }
        else if (!department.Active && department.Id != current?.DepartmentId)
        {
            // Setor desativado não ganha equipe nova; a que já estava nele pode
            // continuar onde está.
            ModelState.AddModelError("departmentId", "This department is inactive.");
        }

        if (!ModelState.IsValid) return;

        // Espelha o índice único (name, department_id, active_key): o nome só
        // não pode se repetir no mesmo setor entre equipes com o mesmo
        // DeletedAt — na prática, entre as ativas.
        var ignoreId = current?.Id ?? 0;
        var deletedAt = current?.DeletedAt;

        if (await db.Team.AnyAsync(t =>
                t.Id != ignoreId &&
                t.Name == name &&
                t.DepartmentId == departmentId &&
                t.DeletedAt == deletedAt))
        {
            ModelState.AddModelError("name", "This department already has a team with this name.");
        }
    }

    // Mesmo formato de data da tela ("02 Oct 2026"), para a mensagem não
    // misturar dois jeitos de escrever a mesma coisa.
    private static string Format(DateOnly date) =>
        date.ToString("dd MMM yyyy", CultureInfo.InvariantCulture);
}
