using System;
using System.Collections.Generic;

namespace NoPrumo.Domain.Entities;

public class TeamProject
{
    public long TeamId { get; set; }

    public long ProjectId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public bool IsOpen() => EndDate is null;

    // Um período novo da mesma equipe na mesma obra não pode cruzar este:
    // o aberto ainda não terminou, e o fechado que acaba no dia da nova
    // entrada (ou depois) se sobrepõe a ela.
    public bool Overlaps(DateOnly startDate) => EndDate is null || EndDate >= startDate;

    public bool EndsBeforeStart(DateOnly endDate) => endDate < StartDate;

    public void End(DateOnly endDate) => EndDate = endDate;
}
