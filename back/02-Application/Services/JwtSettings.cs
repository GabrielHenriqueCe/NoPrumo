namespace NoPrumo.Application.Services;

/// <summary>Configuração do token, preenchida a partir de User Secrets.</summary>
public sealed class JwtSettings
{
    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpirationHours { get; init; } = 8;
}