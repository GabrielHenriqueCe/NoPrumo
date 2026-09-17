using System.ComponentModel.DataAnnotations;

namespace NoPrumo.Application.DTOs;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "Enter your username.")]
    public string Username { get; init; } = string.Empty;

    [Required(ErrorMessage = "Enter your password.")]
    public string Password { get; init; } = string.Empty;
}