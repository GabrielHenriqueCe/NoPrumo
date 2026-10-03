using System.Text.Json.Serialization;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.DTOs;

/// <summary>
/// Obra como todo perfil enxerga: sem nenhum valor em dinheiro.
/// </summary>
[JsonDerivedType(typeof(ProjectFinanceDto))]
public record ProjectDto
{
    public required long Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required long? ClientId { get; init; }
    public required string? ClientName { get; init; }
    public required string? Cno { get; init; }
    public required string? Street { get; init; }
    public required string? Number { get; init; }
    public required string? Complement { get; init; }
    public required string? District { get; init; }
    public required string? City { get; init; }
    public required string? State { get; init; }
    public required string? PostalCode { get; init; }
    public required ProjectStatus Status { get; init; }
    public required long? SupervisorId { get; init; }
    public required string? SupervisorName { get; init; }
    public required string? TechnicalManager { get; init; }
    public required string? CreaRt { get; init; }
    public required DateOnly? StartDate { get; init; }
    public required DateOnly? ForecastDate { get; init; }
    public required DateOnly? CompletionDate { get; init; }
    public required string? Notes { get; init; }
    public required bool Late { get; init; }
    public required bool Active { get; init; }
}
