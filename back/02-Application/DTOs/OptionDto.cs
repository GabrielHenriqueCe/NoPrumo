namespace NoPrumo.Application.DTOs;

public sealed record OptionDto
{
    public required long Id { get; init; }
    public required string Name { get; init; }
}
