using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Services;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(AppDbContext db, TokenService tokens) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await LoadWithRoleAsync(u => u.Username == request.Username);

        // Mesma resposta para usuário inexistente e senha errada: dizer qual
        // dos dois falhou entrega metade da credencial a quem está tentando.
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Problem(statusCode: 401, detail: "Invalid username or password.");
        }

        if (user.Active != true)
        {
            return Problem(statusCode: 403, detail: "This account is inactive. Talk to the administration.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var dto = UserDto.FromEntity(user);

        return Ok(new LoginResponse(tokens.Create(dto), dto));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        if (!TryGetUserId(out var userId))
        {
            return Problem(statusCode: 401, detail: "Invalid token.");
        }

        // O token diz quem é; o banco diz se ainda vale. Uma conta desativada
        // há cinco minutos continua com token válido por horas.
        var user = await LoadWithRoleAsync(u => u.Id == userId);

        if (user is null || user.Active != true)
        {
            return Problem(statusCode: 401, detail: "Your session has expired.");
        }

        return Ok(UserDto.FromEntity(user));
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        if (!TryGetUserId(out var userId))
        {
            return Problem(statusCode: 401, detail: "Invalid token.");
        }

        var user = await db.User.SingleOrDefaultAsync(u => u.Id == userId);

        if (user is null || user.Active != true)
        {
            return Problem(statusCode: 401, detail: "Your session has expired.");
        }

        // Exigir a senha atual impede que um token roubado vire dono da conta.
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            ModelState.AddModelError("currentPassword", "Current password does not match.");
            return ValidationProblem(ModelState);
        }

        if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
        {
            ModelState.AddModelError("newPassword", "The new password must be different.");
            return ValidationProblem(ModelState);
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
        user.MustChangePassword = false;
        await db.SaveChangesAsync();

        return NoContent();
    }

    private Task<Domain.Entities.User?> LoadWithRoleAsync(
        System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>> filter) =>
        db.User
            .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
            .SingleOrDefaultAsync(filter);

    private bool TryGetUserId(out long userId)
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return long.TryParse(subject, out userId);
    }
}