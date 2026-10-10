namespace NoPrumo.Application.DTOs;

public sealed record PortalProjectDto(
    string Name,
    string? City,
    string Status,
    decimal ProgressPercentage,
    DateOnly? ForecastDate,
    IReadOnlyList<PortalStageDto> Stages);
