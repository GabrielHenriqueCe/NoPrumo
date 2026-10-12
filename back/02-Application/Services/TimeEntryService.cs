using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.DTOs.Requests;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Services;

public class TimeEntryService(ITimeEntryRepository repository) : ITimeEntryService
{
    public async Task<TimeEntryDto> SaveAsync(CreateTimeEntryRequest request, long userId, bool canViewFinance, CancellationToken cancellationToken)
    {
        if (!await repository.ActiveProjectExistsAsync(request.ProjectId, cancellationToken))
            throw new ArgumentException("Projeto inativo ou inexistente.");

        var employee = await repository.GetEmployeeAsync(request.EmployeeId, cancellationToken);
        if (employee == null || employee.Active == false)
            throw new ArgumentException("Funcionário inativo ou inexistente.");

        // Vai buscar os encargos válidos no dia do ponto
        var activeCharge = employee.EmploymentRegime?.PayrollChargeRates
            .Where(r => r.EffectiveStart <= request.Date && (r.EffectiveEnd == null || r.EffectiveEnd >= request.Date))
            .OrderByDescending(r => r.EffectiveStart)
            .FirstOrDefault();

        var timeEntry = new TimeEntry
        {
            EmployeeId = request.EmployeeId,
            ProjectId = request.ProjectId,
            Date = request.Date,
            ClockIn = request.ClockIn,
            BreakStart = request.BreakStart,
            BreakEnd = request.BreakEnd,
            ClockOut = request.ClockOut,
            Source = "manual",
            Notes = request.Notes,
            RecordedBy = userId,

            // Congela o salário atual no snapshot!
            HourlyRateSnapshot = employee.PayRate,
            AdditionalSnapshot = employee.AdditionalPercentage,
            ChargesSnapshot = activeCharge?.Percentage ?? 0m
        };

        // Calcula horas e custo gerado
        timeEntry.Hours = timeEntry.CalculateHours();
        timeEntry.Cost = timeEntry.CalculateCost();

        repository.Add(timeEntry);
        await repository.SaveChangesAsync(cancellationToken);

        return MapToDto(timeEntry, canViewFinance);
    }

    public async Task<PagedResult<TimeEntryDto>> ListAsync(int page, int size, long projectId, DateOnly from, DateOnly to, bool canViewFinance, CancellationToken cancellationToken)
    {
        var result = await repository.ListAsync(page, size, projectId, from, to, cancellationToken);

        var dtos = result.Items.Select(t => MapToDto(t, canViewFinance)).ToList();

        return new PagedResult<TimeEntryDto>(dtos, result.Page, result.Size, result.Total, result.TotalPages);
    }

    private static TimeEntryDto MapToDto(TimeEntry timeEntry, bool canViewFinance)
    {
        return new TimeEntryDto
        {
            Id = timeEntry.Id,
            EmployeeId = timeEntry.EmployeeId,
            EmployeeName = timeEntry.Employee?.Name ?? string.Empty,
            ProjectId = timeEntry.ProjectId,
            Date = timeEntry.Date,
            ClockIn = timeEntry.ClockIn,
            BreakStart = timeEntry.BreakStart,
            BreakEnd = timeEntry.BreakEnd,
            ClockOut = timeEntry.ClockOut,
            Hours = timeEntry.Hours,
            Source = timeEntry.Source,
            Notes = timeEntry.Notes,
            // Corta o custo se o utilizador não tiver permissão view_finance
            Cost = canViewFinance ? timeEntry.Cost : null
        };
    }
}