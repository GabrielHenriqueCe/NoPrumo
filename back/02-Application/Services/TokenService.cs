using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Services;

/// <summary>
/// Emite o token que o front guarda e reenvia a cada chamada.
/// </summary>
public sealed class TokenService(JwtSettings settings)
{
    public string Create(UserDto user)
    {
        var claims = new List<Claim>
          {
              new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
              new(JwtRegisteredClaimNames.UniqueName, user.Username),
              new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new(ClaimTypes.Role, user.RoleName),
          };

        // Uma claim por permissão — a autorização pergunta o que pode fazer, não quem é.
        claims.AddRange(user.Permissions.Select(code => new Claim("permission", code)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(settings.ExpirationHours),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}