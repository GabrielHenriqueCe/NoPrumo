using System.Security.Cryptography;

namespace NoPrumo.Application.Services;

public static class PasswordGenerator
{
    // Sem I, l, 1, O e 0 — a senha vai ser ditada por telefone.
    private const string Alphabet =
        "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";

    public static string Create(int length = 12) =>
        RandomNumberGenerator.GetString(Alphabet, length);
}