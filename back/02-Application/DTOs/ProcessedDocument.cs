namespace NoPrumo.Application.DTOs;

public sealed record ProcessedDocument(
    byte[]? Encrypted,
    string? Hash,
    string? Masked);
