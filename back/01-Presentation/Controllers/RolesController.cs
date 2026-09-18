using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Os papéis existentes, para preencher o combo da tela de usuários.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_users")]
public sealed class RolesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> List() =>
        Ok(await db.Role
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .Select(r => new RoleDto(r.Id, r.Name, r.Description ?? r.Name))
            .ToListAsync());
}