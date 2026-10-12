namespace NoPrumo.Application.Results;

public sealed record ProcessedDocument(
    byte[]? Encrypted,
    string? Hash,
    string? Masked);
