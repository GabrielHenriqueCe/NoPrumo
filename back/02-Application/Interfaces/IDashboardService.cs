using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync(IReadOnlySet<string> permissions, CancellationToken cancellationToken);
}
