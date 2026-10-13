using NoPrumo.Application.Access;
using NoPrumo.Application.DTOs;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Interfaces;

public interface IProjectRepository
{
    // Client, Supervisor e Stages vêm carregados: o service precisa dos nomes e do avanço.
    // Obra fora do alcance de quem pergunta não aparece na lista e volta null no GetById.
    Task<PagedResult<Project>> ListAsync(int page, int size, string? search, ProjectAccess access, CancellationToken cancellationToken);
    Task<Project?> GetByIdAsync(long id, ProjectAccess access, CancellationToken cancellationToken);
    // Sem alcance: para quem já validou a permissão por outro caminho (link do portal, compras).
    Task<Project?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Project?> GetForUpdateAsync(long id, CancellationToken cancellationToken);
    Task<bool> CodeExistsAsync(string code, long? ignoredProjectId, CancellationToken cancellationToken);
    void Add(Project project);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
