using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace NoPrumo.Application.Services;

public sealed record ProcessedDocument(
    byte[]? Encrypted,
    string? Hash,
    string? Masked);

public static class DocumentProcessor
{
    public static ProcessedDocument Process(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            return new ProcessedDocument(null, null, null);
        }

        var digits = Regex.Replace(rawInput, @"\D", "");
        if (string.IsNullOrEmpty(digits))
        {
            return new ProcessedDocument(null, null, null);
        }

        var bytes = Encoding.UTF8.GetBytes(digits);
        var hash = Convert.ToHexString(SHA256.HashData(bytes));

        var masked = MaskDocument(digits);

        return new ProcessedDocument(bytes, hash, masked);
    }

    public static string MaskDocument(string digits)
    {
        if (digits.Length == 11)
        {
            // CPF: ***.456.789-**
            return $"***.{digits.Substring(3, 3)}.{digits.Substring(6, 3)}-**";
        }

        if (digits.Length == 14)
        {
            // CNPJ: **.345.678/0001-**
            return $"**.{digits.Substring(2, 3)}.{digits.Substring(5, 3)}/{digits.Substring(8, 4)}-**";
        }

        if (digits.Length > 4)
        {
            var len = digits.Length;
            return $"{new string('*', len - 4)}{digits.Substring(len - 4)}";
        }

        return new string('*', digits.Length);
    }
}
