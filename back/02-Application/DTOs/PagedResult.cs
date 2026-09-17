namespace NoPrumo.Application.DTOs;

/// <summary>
/// Uma página de resultados. Listas como stock_movement e time_entry crescem
/// rápido demais para devolver inteiras.
/// </summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int Size,
    int Total,
    int TotalPages);