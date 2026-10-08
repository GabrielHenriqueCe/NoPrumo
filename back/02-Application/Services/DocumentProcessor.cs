using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Services;

public class DocumentProcessor(DocumentSettings documentSettings)
{
    public ProcessedDocument Process(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            return new ProcessedDocument(null, null, null);
        }

        var cleaned = CleanDocument(rawInput);
        if (string.IsNullOrEmpty(cleaned))
        {
            return new ProcessedDocument(null, null, null);
        }

        var encrypted = EncryptAesGcm(cleaned, documentSettings.EncryptionKey);
        var hash = ComputeHmacSha256(cleaned, documentSettings.HmacKey);
        var masked = MaskDocument(cleaned);

        return new ProcessedDocument(encrypted, hash, masked);
    }

    public static string CleanDocument(string rawInput)
    {
        return Regex.Replace(rawInput, @"[^a-zA-Z0-9]", "").ToUpperInvariant();
    }

    public static bool IsValid(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput)) return true;
        var cleaned = CleanDocument(rawInput);
        if (string.IsNullOrEmpty(cleaned)) return true;

        if (cleaned.Length == 11) return IsValidCpf(cleaned);
        if (cleaned.Length == 14) return IsValidCnpj(cleaned);

        return false;
    }

    public static bool IsValidCpf(string cpf)
    {
        if (cpf.Length != 11) return false;
        if (!Regex.IsMatch(cpf, @"^\d{11}$")) return false;
        if (new string(cpf[0], 11) == cpf) return false;

        int[] multiplierFirstDigit = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplierSecondDigit = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        string tempCpf = cpf.Substring(0, 9);
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (tempCpf[i] - '0') * multiplierFirstDigit[i];

        int remainder = sum % 11;
        int firstDigit = remainder < 2 ? 0 : 11 - remainder;

        tempCpf += firstDigit;
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (tempCpf[i] - '0') * multiplierSecondDigit[i];

        remainder = sum % 11;
        int secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return cpf.EndsWith($"{firstDigit}{secondDigit}");
    }

    public static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14) return false;
        if (!Regex.IsMatch(cnpj, @"^[A-Z0-9]{12}\d{2}$")) return false;
        if (new string(cnpj[0], 14) == cnpj) return false;

        int[] multiplierFirstDigit = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplierSecondDigit = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        string tempCnpj = cnpj.Substring(0, 12);
        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            char charVal = tempCnpj[i];
            int charAsciiValue = charVal - '0';
            sum += charAsciiValue * multiplierFirstDigit[i];
        }

        int remainder = sum % 11;
        int firstDigit = remainder < 2 ? 0 : 11 - remainder;

        tempCnpj += firstDigit;
        sum = 0;
        for (int i = 0; i < 13; i++)
        {
            char charVal = tempCnpj[i];
            int charAsciiValue = charVal - '0';
            sum += charAsciiValue * multiplierSecondDigit[i];
        }

        remainder = sum % 11;
        int secondDigit = remainder < 2 ? 0 : 11 - remainder;

        return cnpj.EndsWith($"{firstDigit}{secondDigit}");
    }

    public static byte[] EncryptAesGcm(string plainText, byte[] key)
    {
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] nonce = new byte[12];
        RandomNumberGenerator.Fill(nonce);

        byte[] ciphertext = new byte[plainBytes.Length];
        byte[] tag = new byte[16];

        using var aesGcm = new AesGcm(key, tagSizeInBytes: 16);
        aesGcm.Encrypt(nonce, plainBytes, ciphertext, tag);

        byte[] result = new byte[12 + 16 + ciphertext.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, 12);
        Buffer.BlockCopy(tag, 0, result, 12, 16);
        Buffer.BlockCopy(ciphertext, 0, result, 28, ciphertext.Length);

        return result;
    }

    public static string DecryptAesGcm(byte[] encryptedPayload, byte[] key)
    {
        if (encryptedPayload == null || encryptedPayload.Length < 28) return string.Empty;

        byte[] nonce = new byte[12];
        byte[] tag = new byte[16];
        byte[] ciphertext = new byte[encryptedPayload.Length - 28];

        Buffer.BlockCopy(encryptedPayload, 0, nonce, 0, 12);
        Buffer.BlockCopy(encryptedPayload, 12, tag, 0, 16);
        Buffer.BlockCopy(encryptedPayload, 28, ciphertext, 0, ciphertext.Length);

        byte[] plainBytes = new byte[ciphertext.Length];

        using var aesGcm = new AesGcm(key, tagSizeInBytes: 16);
        aesGcm.Decrypt(nonce, ciphertext, tag, plainBytes);

        return Encoding.UTF8.GetString(plainBytes);
    }

    public static string ComputeHmacSha256(string plainText, byte[] hmacKey)
    {
        using var hmac = new HMACSHA256(hmacKey);
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        return Convert.ToHexString(hashBytes);
    }

    public static string MaskDocument(string cleanedDocument)
    {
        if (cleanedDocument.Length == 11)
        {
            // CPF: ***.456.789-**
            return $"***.{cleanedDocument.Substring(3, 3)}.{cleanedDocument.Substring(6, 3)}-**";
        }

        if (cleanedDocument.Length == 14)
        {
            // CNPJ: **.345.678/0001-**
            return $"**.{cleanedDocument.Substring(2, 3)}.{cleanedDocument.Substring(5, 3)}/{cleanedDocument.Substring(8, 4)}-**";
        }

        if (cleanedDocument.Length > 4)
        {
            var len = cleanedDocument.Length;
            return $"{new string('*', len - 4)}{cleanedDocument.Substring(len - 4)}";
        }

        return new string('*', cleanedDocument.Length);
    }
}
