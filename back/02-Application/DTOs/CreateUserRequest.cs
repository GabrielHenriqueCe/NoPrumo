using System.ComponentModel.DataAnnotations;

namespace NoPrumo.Application.DTOs;

public sealed class CreateUserRequest
{
    [Required(ErrorMessage = "Enter the full name.")]
    [MaxLength(160)]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Enter the username.")]
    [MaxLength(80)]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Use letters, numbers, dot, dash or underscore only.")]
    public string Username { get; init; } = string.Empty;

    [EmailAddress(ErrorMessage = "This email is not valid.")]
    [MaxLength(160)]
    public string? Email { get; init; }

    [Range(1, long.MaxValue, ErrorMessage = "Pick a role.")]
    public long RoleId { get; init; }
}