using System;
using System.Collections.Generic;

using System.Linq;

namespace NoPrumo.Domain.Entities;

public class Employee
{
    public long Id { get; set; }

    public string? RegistrationNumber { get; set; }

    public string Name { get; set; } = null!;

    public long? JobRoleId { get; set; }

    public long? EmploymentRegimeId { get; set; }

    public decimal PayRate { get; set; }

    public decimal AdditionalPercentage { get; set; }

    public DateOnly? HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public string? Phone { get; set; }

    public byte[]? DocumentEncrypted { get; set; }

    public string? DocumentHash { get; set; }

    public string? DocumentMasked { get; set; }

    public bool? Active { get; set; } = true;

    public DateTime? AnonymizedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? ActiveKey { get; set; }

    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

    public virtual ICollection<Stage> Stages { get; set; } = new List<Stage>();

    public virtual ICollection<PpeIssue> PpeIssues { get; set; } = new List<PpeIssue>();

    public virtual JobRole? JobRole { get; set; }

    public virtual ICollection<EmployeeTraining> EmployeeTrainings { get; set; } = new List<EmployeeTraining>();

    public virtual ICollection<EmployeeTeam> EmployeeTeams { get; set; } = new List<EmployeeTeam>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    public virtual EmploymentRegime? EmploymentRegime { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    // Só o certificado mais recente de cada tipo vale: um NR-35 vencido que já
    // foi renovado não está mais vencendo. Exige EmployeeTrainings carregado.
    public IEnumerable<EmployeeTraining> LatestTrainings() =>
        EmployeeTrainings
            .GroupBy(training => training.TrainingTypeId)
            .Select(trainings => trainings
                .OrderByDescending(training => training.IssueDate)
                .ThenByDescending(training => training.Id)
                .First());

    // Confirmação da entrega de EPI: o nome digitado tem que ser o do cadastro,
    // sem diferença de maiúscula, acento ou espaço sobrando.
    public bool IsNamed(string typedName) => Normalize(typedName) == Normalize(Name);

    private static string Normalize(string name)
    {
        var withoutAccents = new string(name
            .Normalize(System.Text.NormalizationForm.FormD)
            .Where(character => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(character)
                != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray());

        return string.Join(' ', withoutAccents.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToUpperInvariant();
    }
}
