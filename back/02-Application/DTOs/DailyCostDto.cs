namespace NoPrumo.Application.DTOs;

public sealed record DailyCostDto(
    DateOnly Date,
    decimal Material,
    decimal Labor,
    decimal CumulativeCost);
