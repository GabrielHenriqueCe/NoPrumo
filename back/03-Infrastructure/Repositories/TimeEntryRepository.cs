using Microsoft.EntityFrameworkCore;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Infrastructure.Data;

namespace NoPrumo.Infrastructure.Repositories;

public class TimeEntryRepository(AppDbContext appDbContext) : ITimeEntryRepository
{
    public async Task<PagedResult<TimeEntry>> ListAsync(int page, int size, long projectId, DateOnly from, DateOnly to, CancellationToken cancellationToken)
    {
        var query = appDbContext.TimeEntry
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId && t.Date >= from && t.Date <= to);

        var total = await query.CountAsync(cancellationToken);

        var entries = await query
            .Include(t => t.Employee)
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.ClockIn) // Baseado nas propriedades do seu modelo original
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));

        return new PagedResult<TimeEntry>(entries, page, size, total, totalPages);
    }

    public Task<TimeEntry?> GetByIdAsync(long id, CancellationToken cancellationToken) =>
        appDbContext.TimeEntry
            .AsNoTracking()
            .Include(t => t.Employee)
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<Employee?> GetEmployeeAsync(long employeeId, CancellationToken cancellationToken) =>
        appDbContext.Employee
            .AsNoTracking()
            .Include(e => e.EmploymentRegime)
                .ThenInclude(r => r.PayrollChargeRates) // Precisamos disto para o Snapshot do custo!
            .FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

    public Task<bool> ActiveProjectExistsAsync(long projectId, CancellationToken cancellationToken) =>
        appDbContext.Project.AnyAsync(p => p.Id == projectId && p.DeletedAt == null, cancellationToken);

    public void Add(TimeEntry timeEntry) =>
        appDbContext.TimeEntry.Add(timeEntry);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        appDbContext.SaveChangesAsync(cancellationToken);
}