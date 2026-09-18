using System.ComponentModel.DataAnnotations;

namespace NoPrumo.Application.DTOs;

public sealed class ChangePasswordRequest
{
    [Required(ErrorMessage = "Enter your current password.")]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required(ErrorMessage = "Enter the new password.")]
    [MinLength(8, ErrorMessage = "Use at least 8 characters.")]
    public string NewPassword { get; init; } = string.Empty;
}