using System;
using System.Collections.Generic;

using NoPrumo.Domain.Enums;

namespace NoPrumo.Domain.Entities;

public class Appointment
{
    public long Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? Time { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public long? ProjectId { get; set; }

    public long? AssignedToId { get; set; }

    public AppointmentType Type { get; set; }

    public bool Completed { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Project? Project { get; set; }

    public virtual User? AssignedTo { get; set; }

    // Concluir de novo não mexe na data: vale a primeira conclusão.
    public void Complete(DateTime now)
    {
        Completed = true;
        CompletedAt ??= now;
    }

    public void Reopen()
    {
        Completed = false;
        CompletedAt = null;
    }
}
