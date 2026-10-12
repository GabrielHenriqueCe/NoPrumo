using System.Security.Claims;
using NoPrumo.Application.Access;

namespace NoPrumo.Extensions;

public static class ClaimsPrincipalExtensions
{
    // As duas claims saem do TokenService no login: quem ganha uma permissão nova precisa sair e entrar.
    public static ProjectAccess ProjectAccess(this ClaimsPrincipal user)
    {
        var employeeId = long.TryParse(user.FindFirst("employee_id")?.Value, out var id) ? id : (long?)null;
        return new ProjectAccess(user.HasClaim("permission", "view_all_projects"), employeeId);
    }
}
