namespace NoPrumo.Application.Requests;

public sealed record CreateProjectLinkRequest(
    string? Label,
    int? DaysValid);
