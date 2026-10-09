namespace NoPrumo.Application.DTOs;

/// <summary>
/// A categoria de estoque como o front a recebe. As duas flags definem o fluxo
/// do sistema inteiro: consumo (saldo por obra), EPI (depósito → funcionário)
/// e ferramenta (vai e volta).
/// </summary>
public sealed record StockCategoryDto(
    long Id,
    string Name,
    bool TracksProjectBalance,
    bool RequiresReturn);