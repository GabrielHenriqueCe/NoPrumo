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
    public static ProcessedDocument Process(string? rawInput, byte[] encryptionKey, byte[] hmacKey)
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

        var encrypted = EncryptAesGcm(digits, encryptionKey);
        var hash = ComputeHmacSha256(digits, hmacKey);
        var masked = MaskDocument(digits);

        return new ProcessedDocument(encrypted, hash, masked);
    }

    public static bool IsValid(string? rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput)) return true;
        var digits = Regex.Replace(rawInput, @"\D", "");
        if (string.IsNullOrEmpty(digits)) return true;

        if (digits.Length == 11) return IsValidCpf(digits);
        if (digits.Length == 14) return IsValidCnpj(digits);

        return false;
    }

    public static bool IsValidCpf(string cpf)
    {
        if (cpf.Length != 11) return false;
        if (new string(cpf[0], 11) == cpf) return false;

        int[] mult1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] mult2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        string tempCpf = cpf.Substring(0, 9);
        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += (tempCpf[i] - '0') * mult1[i];

        int rem = sum % 11;
        int d1 = rem < 2 ? 0 : 11 - rem;

        tempCpf += d1;
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += (tempCpf[i] - '0') * mult2[i];

        rem = sum % 11;
        int d2 = rem < 2 ? 0 : 11 - rem;

        return cpf.EndsWith($"{d1}{d2}");
    }

    public static bool IsValidCnpj(string cnpj)
    {
        if (cnpj.Length != 14) return false;
        if (new string(cnpj[0], 14) == cnpj) return false;

        int[] mult1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] mult2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        string tempCnpj = cnpj.Substring(0, 12);
        int sum = 0;
        for (int i = 0; i < 12; i++)
            sum += (tempCnpj[i] - '0') * mult1[i];

        int rem = sum % 11;
        int d1 = rem < 2 ? 0 : 11 - rem;

        tempCnpj += d1;
        sum = 0;
        for (int i = 0; i < 13; i++)
            sum += (tempCnpj[i] - '0') * mult2[i];

        rem = sum % 11;
        int d2 = rem < 2 ? 0 : 11 - rem;

        return cnpj.EndsWith($"{d1}{d2}");
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
