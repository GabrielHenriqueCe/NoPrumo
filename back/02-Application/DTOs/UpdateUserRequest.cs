using System.ComponentModel.DataAnnotations;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// O username não está aqui de propósito: ele identifica quem assinou cada
/// lançamento, e renomear depois deixaria o histórico órfão.
/// </summary>
public sealed class UpdateUserRequest
{
    [Required(ErrorMessage = "Enter the full name.")]
    [MaxLength(160)]
    public string Name { get; init; } = string.Empty;

    [EmailAddress(ErrorMessage = "This email is not valid.")]
    [MaxLength(160)]
    public string? Email { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Pick a role.")]
    public long RoleId { get; init; }
}