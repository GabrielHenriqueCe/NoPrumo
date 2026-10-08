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
public sealed class RolesController(AppDbContext appDbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> List() =>
        Ok(await appDbContext.Role
            .AsNoTracking()
            .OrderBy(role => role.Id)
            .Select(role => new RoleDto(role.Id, role.Name, role.Description ?? role.Name))
            .ToListAsync());
}
