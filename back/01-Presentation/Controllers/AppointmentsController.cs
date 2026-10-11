using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "manage_schedule")]
public sealed class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AgendaDto>> GetAgenda(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] long? projectId,
        CancellationToken cancellationToken)
    {
        var result = await appointmentService.GetAgendaAsync(from, to, projectId, cancellationToken);
        return result.Succeeded ? Ok(result.Agenda) : ValidationProblemFrom(result.Errors);
    }

    [HttpGet("options")]
    public async Task<ActionResult<AgendaOptionsDto>> GetOptions(CancellationToken cancellationToken) =>
        Ok(await appointmentService.GetOptionsAsync(cancellationToken));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AppointmentDto>> GetById(long id, CancellationToken cancellationToken)
    {
        var appointment = await appointmentService.GetByIdAsync(id, cancellationToken);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create(SaveAppointmentRequest request, CancellationToken cancellationToken)
    {
        var result = await appointmentService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Appointment!.Id }, result.Appointment);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AppointmentDto>> Update(long id, SaveAppointmentRequest request, CancellationToken cancellationToken)
    {
        var result = await appointmentService.UpdateAsync(id, request, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        return result.Succeeded ? Ok(result.Appointment) : ValidationProblemFrom(result.Errors);
    }

    [HttpPatch("{id:long}/complete")]
    public async Task<ActionResult<AppointmentDto>> Complete(long id, CancellationToken cancellationToken)
    {
        var appointment = await appointmentService.CompleteAsync(id, cancellationToken);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    [HttpPatch("{id:long}/reopen")]
    public async Task<ActionResult<AppointmentDto>> Reopen(long id, CancellationToken cancellationToken)
    {
        var appointment = await appointmentService.ReopenAsync(id, cancellationToken);
        return appointment is null ? NotFound() : Ok(appointment);
    }

    private ActionResult ValidationProblemFrom(IReadOnlyDictionary<string, string> errors)
    {
        foreach (var (field, message) in errors)
        {
            ModelState.AddModelError(field, message);
        }

        return ValidationProblem(ModelState);
    }
}
