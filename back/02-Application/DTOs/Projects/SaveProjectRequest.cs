using System.ComponentModel.DataAnnotations;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs.Projects;

public sealed record SaveProjectRequest
{
    [Required(ErrorMessage = "Enter the project code.")]
    [MaxLength(50)]
    public string Code { get; init; } = string.Empty;

    [Required(ErrorMessage = "Enter the project name.")]
    [MaxLength(180)]
    public string Name { get; init; } = string.Empty;

    public long? ClientId { get; init; }

    public ProjectStatus Status { get; init; } = ProjectStatus.Planning;

    public long? SupervisorId { get; init; }

    [MaxLength(30)]
    public string? Cno { get; init; }

    [MaxLength(160)]
    public string? TechnicalManager { get; init; }

    [MaxLength(40)]
    public string? CreaRt { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? ForecastDate { get; init; }

    [MaxLength(255)]
    public string? Street { get; init; }

    [MaxLength(20)]
    public string? Number { get; init; }

    [MaxLength(100)]
    public string? Complement { get; init; }

    [MaxLength(100)]
    public string? District { get; init; }

    [MaxLength(120)]
    public string? City { get; init; }

    [StringLength(2, MinimumLength = 2, ErrorMessage = "Use the two-letter state code.")]
    public string? State { get; init; }

    [MaxLength(10)]
    public string? PostalCode { get; init; }

    [Range(typeof(decimal), "0", "9999999999999.99", ErrorMessage = "Enter an amount of zero or more.")]
    public decimal? ContractAmount { get; init; }
}