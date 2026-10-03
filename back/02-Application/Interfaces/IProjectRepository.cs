using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IProjectRepository
{
    // Client e Supervisor vêm carregados: o service precisa dos nomes.
    Task<PagedResult<Project>> ListAsync(int page, int size, string? search, CancellationToken cancellationToken);

    Task<Project?> GetByIdAsync(long id, CancellationToken cancellationToken);
}