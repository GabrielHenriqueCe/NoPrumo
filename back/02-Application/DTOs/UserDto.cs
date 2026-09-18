using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// O usuário como o front o recebe. PasswordHash não tem propriedade aqui —
/// é assim que ele nunca vaza por esquecimento.
/// </summary>
public sealed record UserDto(
    long Id,
    string Username,
    string Name,
    string? Email,
    long RoleId,
    string RoleName,
    string RoleLabel,
    IReadOnlyList<string> Permissions,
    bool Active,
    bool MustChangePassword,
    DateTime? LastLoginAt)
{
    /// <summary>
    /// Exige que o User venha com Role e Role.Permissions carregados
    /// (.Include(u => u.Role).ThenInclude(r => r.Permissions)).
    /// </summary>
    public static UserDto FromEntity(User user) => new(
        user.Id,
        user.Username,
        user.Name,
        user.Email,
        user.RoleId,
        user.Role.Name,
        user.Role.Description ?? user.Role.Name,
        user.Role.Permissions.Select(p => p.Code).ToArray(),
        user.Active ?? true,
        user.MustChangePassword,
        user.LastLoginAt);
}