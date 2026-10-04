using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NoPrumo.Infrastructure.Data.Converters;

/// <summary>
/// Grava qualquer enum como texto em snake_case: ProjectStatus.InProgress
/// vira 'in_progress' no banco, que é a convenção dos valores gravados.
/// </summary>
public class SnakeCaseEnumConverter<TEnum> : ValueConverter<TEnum, string>
    where TEnum : struct, Enum
{
    public SnakeCaseEnumConverter()
        : base(
            value => JsonNamingPolicy.SnakeCaseLower.ConvertName(value.ToString()),
            text => Enum.Parse<TEnum>(text.Replace("_", ""), true))
    {
    }
}