namespace NoPrumo.Application.Services;

/// <summary>
/// Chaves criptográficas para processamento de documentos (CPF/CNPJ).
/// Lidas de User Secrets — nunca hardcoded.
/// </summary>
public sealed class DocumentSettings
{
    /// <summary>Chave AES-256-GCM (32 bytes) para cifrar o documento.</summary>
    public required byte[] EncryptionKey { get; init; }

    /// <summary>Chave HMAC-SHA-256 (32 bytes) para hash de busca.</summary>
    public required byte[] HmacKey { get; init; }
}
