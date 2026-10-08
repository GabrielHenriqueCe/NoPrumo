using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Responses;
using NoPrumo.Application.Services;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

/// <summary>
/// Gestão de usuários. Não existe auto-cadastro: a conta nasce aqui e a API
/// devolve uma senha provisória para o administrador entregar.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_users")]
public sealed class UsersController(AppDbContext appDbContext, PasswordGenerator passwordGenerator) : ControllerBase
{
    private const int MaxPageSize = 100;

    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null)
    {
        // Vem da query string, então vem de fora: um size=100000 derrubaria a
        // consulta, e um page=-5 quebraria o Skip.
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var query = appDbContext.User
            .AsNoTracking()
            .Include(user => user.Role)
                .ThenInclude(role => role.Permissions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query = query.Where(user =>
                user.Name.Contains(term) ||
                user.Username.Contains(term) ||
                (user.Email != null && user.Email.Contains(term)));
        }

        // Conta antes de paginar: o total é do filtro inteiro, não da página.
        var total = await query.CountAsync();

        var items = await query
            .OrderBy(user => user.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PagedResult<UserDto>(
            items.Select(UserDto.FromEntity).ToArray(),
            page,
            size,
            total,
            Math.Max(1, (int)Math.Ceiling(total / (double)size))));
    }

    [HttpPost]
    public async Task<ActionResult<CreatedUserResponse>> Create(CreateUserRequest request)
    {
        var username = request.Username.Trim().ToLowerInvariant();
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

        if (await appDbContext.User.AnyAsync(existingUser => existingUser.Username == username))
        {
            ModelState.AddModelError("username", "This username is already taken.");
            return ValidationProblem(ModelState);
        }

        if (email is not null && await appDbContext.User.AnyAsync(existingUser => existingUser.Email == email))
        {
            ModelState.AddModelError("email", "This email is already in use.");
            return ValidationProblem(ModelState);
        }

        if (!await appDbContext.Role.AnyAsync(role => role.Id == request.RoleId))
        {
            ModelState.AddModelError("roleId", "Pick a role.");
            return ValidationProblem(ModelState);
        }

        // Quem cria não escolhe a senha: escolher significaria conhecê-la, e a
        // conta deixaria de ser só da pessoa dona dela.
        var temporaryPassword = passwordGenerator.Create();

        var user = new User
        {
            Username = username,
            Name = request.Name.Trim(),
            Email = email,
            RoleId = request.RoleId,
            Active = true,
            MustChangePassword = true,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword, workFactor: 12),
        };

        appDbContext.User.Add(user);
        await appDbContext.SaveChangesAsync();

        var created = await LoadDtoAsync(user.Id);

        return StatusCode(
            StatusCodes.Status201Created,
            new CreatedUserResponse(created!, temporaryPassword));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<UserDto>> Update(long id, UpdateUserRequest request)
    {
        var user = await appDbContext.User.SingleOrDefaultAsync(candidate => candidate.Id == id);

        if (user is null) return NotFound();

        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();

        if (email is not null && await appDbContext.User.AnyAsync(existingUser => existingUser.Email == email && existingUser.Id != id))
        {
            ModelState.AddModelError("email", "This email is already in use.");
            return ValidationProblem(ModelState);
        }

        if (!await appDbContext.Role.AnyAsync(role => role.Id == request.RoleId))
        {
            ModelState.AddModelError("roleId", "Pick a role.");
            return ValidationProblem(ModelState);
        }

        // Rebaixar o próprio papel tira o acesso a esta tela na hora, e não
        // sobra ninguém para desfazer.
        if (id == CurrentUserId() && user.RoleId != request.RoleId)
        {
            ModelState.AddModelError("roleId", "You cannot change your own role.");
            return ValidationProblem(ModelState);
        }

        user.Name = request.Name.Trim();
        user.Email = email;
        user.RoleId = request.RoleId;

        await appDbContext.SaveChangesAsync();

        return Ok(await LoadDtoAsync(id));
    }

    [HttpPatch("{id:long}/activate")]
    public Task<IActionResult> Activate(long id) => SetActiveAsync(id, true);

    [HttpPatch("{id:long}/deactivate")]
    public Task<IActionResult> Deactivate(long id) => SetActiveAsync(id, false);

    [HttpPost("{id:long}/reset-password")]
    public async Task<ActionResult<TemporaryPasswordResponse>> ResetPassword(long id)
    {
        // Resetar a própria senha só gera uma provisória que você precisa
        // anotar. Para trocar a sua existe /auth/change-password.
        if (id == CurrentUserId())
        {
            return Problem(statusCode: 400, detail: "To change your own password, use the change password screen.");
        }

        var user = await appDbContext.User.SingleOrDefaultAsync(candidate => candidate.Id == id);

        if (user is null) return NotFound();

        var temporaryPassword = passwordGenerator.Create();

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword, workFactor: 12);
        user.MustChangePassword = true;

        await appDbContext.SaveChangesAsync();

        return Ok(new TemporaryPasswordResponse(temporaryPassword));
    }

    private async Task<IActionResult> SetActiveAsync(long id, bool active)
    {
        var user = await appDbContext.User.SingleOrDefaultAsync(candidate => candidate.Id == id);

        if (user is null) return NotFound();

        // Desativar a própria conta é trancar a chave do lado de dentro.
        if (!active && id == CurrentUserId())
        {
            return Problem(statusCode: 400, detail: "You cannot deactivate your own account.");
        }

        user.Active = active;
        await appDbContext.SaveChangesAsync();

        return NoContent();
    }

    private Task<UserDto?> LoadDtoAsync(long id) =>
        appDbContext.User
            .AsNoTracking()
            .Include(user => user.Role)
                .ThenInclude(role => role.Permissions)
            .Where(user => user.Id == id)
            .Select(user => UserDto.FromEntity(user))
            .SingleOrDefaultAsync();

    private long? CurrentUserId()
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return long.TryParse(subject, out var id) ? id : null;
    }
}
