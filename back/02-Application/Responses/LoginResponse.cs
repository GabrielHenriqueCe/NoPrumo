using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Responses;

public sealed record LoginResponse(string Token, UserDto User);
