using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Responses;

/// <summary>
/// A senha provisória só existe neste retorno: o banco guarda o hash, então
/// ela não pode ser consultada depois.
/// </summary>
public sealed record CreatedUserResponse(UserDto User, string TemporaryPassword);
