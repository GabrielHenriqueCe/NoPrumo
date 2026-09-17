namespace NoPrumo.Application.DTOs;

public sealed record LoginResponse(string Token, UserDto User);