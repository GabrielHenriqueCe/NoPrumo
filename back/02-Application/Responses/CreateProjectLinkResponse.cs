namespace NoPrumo.Application.Responses;

public sealed record CreateProjectLinkResponse(
    long Id,
    long ProjectId,
    string Token,
    string? Label,
    DateTime? ExpiresAt,
    DateTime CreatedAt);
