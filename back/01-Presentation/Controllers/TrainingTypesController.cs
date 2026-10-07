using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Cadastro dos tipos de treinamento (NR-35, NR-10, ASO...). Não há desativar:
/// a entidade não tem Active/DeletedAt, e apagar quebraria a FK dos
/// treinamentos já registrados.
///
/// Mudar a validade aqui não recalcula os treinamentos que já existem: cada um
/// guardou o vencimento que valia quando foi registrado.
/// </summary>
[ApiController]
[Route("api/[controller]")] // vira /api/trainingtypes
[Authorize(Policy = "manage_safety")]
public sealed class TrainingTypesController(AppDbContext db) : ControllerBase
{
    private const int MaxPageSize = 100;

    // Tetos só para pegar erro de digitação: CNH vale 10 anos, e nenhum
    // curso de NR passa de algumas dezenas de horas.
    private const int MaxValidityMonths = 120;
    private const int MaxWorkloadHours = 1000;

    [HttpGet]
    public async Task<ActionResult<PagedResult<TrainingTypeDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var query = db.TrainingType.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t => t.Code.Contains(term) || t.Name.Contains(term));
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(t => t.Code)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<TrainingTypeDto>(
            items.Select(ToDto).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<TrainingTypeDto>> GetById(long id)
    {
        var type = await db.TrainingType.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);

        if (type is null) return NotFound();

        return Ok(ToDto(type));
    }

    [HttpPost]
    public async Task<ActionResult<TrainingTypeDto>> Create(SaveTrainingTypeRequest request)
    {
        var code = NormalizeCode(request.Code);
        var name = request.Name?.Trim() ?? string.Empty;

        await ValidateAsync(code, name, request.ValidityMonths, request.MinWorkloadHours);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var type = new TrainingType
        {
            Code = code,
            Name = name,
            ValidityMonths = request.ValidityMonths,
            MinWorkloadHours = request.MinWorkloadHours,
            RequiresInPerson = request.RequiresInPerson,
        };

        db.TrainingType.Add(type);
        await db.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, ToDto(type));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<TrainingTypeDto>> Update(long id, SaveTrainingTypeRequest request)
    {
        var type = await db.TrainingType.SingleOrDefaultAsync(t => t.Id == id);

        if (type is null) return NotFound();

        var code = NormalizeCode(request.Code);
        var name = request.Name?.Trim() ?? string.Empty;

        await ValidateAsync(code, name, request.ValidityMonths, request.MinWorkloadHours, id);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        type.Code = code;
        type.Name = name;
        type.ValidityMonths = request.ValidityMonths;
        type.MinWorkloadHours = request.MinWorkloadHours;
        type.RequiresInPerson = request.RequiresInPerson;

        await db.SaveChangesAsync();

        return Ok(ToDto(type));
    }

    // Código de norma se escreve em maiúscula (NR-35, ASO). Normalizar aqui
    // evita que "nr-35" e "NR-35" pareçam dois tipos na lista.
    private static string NormalizeCode(string? code) =>
        code?.Trim().ToUpperInvariant() ?? string.Empty;

    // Valida os campos. Cada erro leva o nome do campo, e é isso que faz a
    // mensagem aparecer embaixo do campo certo na tela.
    private async Task ValidateAsync(
        string code, string name, int? validityMonths, int? minWorkloadHours, long ignoreId = 0)
    {
        if (string.IsNullOrEmpty(code))
        {
            ModelState.AddModelError("code", "Code is required.");
        }
        else if (code.Length > 30)
        {
            ModelState.AddModelError("code", "Use at most 30 characters.");
        }
        else if (await db.TrainingType.AnyAsync(t => t.Code == code && t.Id != ignoreId))
        {
            // O índice único do banco recusaria do mesmo jeito; aqui a recusa
            // chega com o campo, em vez de virar erro 500.
            ModelState.AddModelError("code", "This code is already in use.");
        }

        if (string.IsNullOrEmpty(name))
        {
            ModelState.AddModelError("name", "Name is required.");
        }
        else if (name.Length > 160)
        {
            ModelState.AddModelError("name", "Use at most 160 characters.");
        }

        // O banco não tem CHECK constraint (regra do projeto: a migration sai
        // só do comando), então faixa de valor é obrigação do controller.
        if (validityMonths is < 1 or > MaxValidityMonths)
        {
            ModelState.AddModelError(
                "validityMonths",
                $"Use 1 to {MaxValidityMonths} months, or leave it blank if it never expires.");
        }

        if (minWorkloadHours is < 1 or > MaxWorkloadHours)
        {
            ModelState.AddModelError(
                "minWorkloadHours",
                $"Use 1 to {MaxWorkloadHours} hours, or leave it blank if there is no minimum.");
        }
    }

    private static TrainingTypeDto ToDto(TrainingType trainingType) => new()
    {
        Id = trainingType.Id,
        Code = trainingType.Code,
        Name = trainingType.Name,
        ValidityMonths = trainingType.ValidityMonths,
        MinWorkloadHours = trainingType.MinWorkloadHours,
        RequiresInPerson = trainingType.RequiresInPerson,
    };
}
