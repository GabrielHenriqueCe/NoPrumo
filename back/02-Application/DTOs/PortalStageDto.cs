namespace NoPrumo.Application.DTOs;

public sealed record PortalStageDto(
    string Name,
    decimal Percentage,
    string Status,
    bool Late);
