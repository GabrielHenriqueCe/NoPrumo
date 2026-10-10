namespace NoPrumo.Application.DTOs;

public sealed record ProjectLinkDto(
    long Id,
    long ProjectId,
    string? Label,
    DateTime? ExpiresAt,
    DateTime? RevokedAt,
    DateTime? LastAccessAt,
    int AccessCount,
    DateTime CreatedAt,
    bool IsActive);
