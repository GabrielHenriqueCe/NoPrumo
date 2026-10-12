using System;
using System.Threading;
using System.Threading.Tasks;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Requests;

namespace NoPrumo.Application.Interfaces;

public interface ITimeEntryService
{
    Task<TimeEntryDto> SaveAsync(CreateTimeEntryRequest request, long userId, bool canViewFinance, CancellationToken cancellationToken);
    Task<PagedResult<TimeEntryDto>> ListAsync(int page, int size, long projectId, DateOnly from, DateOnly to, bool canViewFinance, CancellationToken cancellationToken);
    Task<decimal> GetTotalLaborCostAsync(long projectId, CancellationToken cancellationToken);
}