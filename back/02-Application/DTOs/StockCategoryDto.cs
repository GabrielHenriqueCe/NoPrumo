using NoPrumo.Domain.Entities;

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
    bool RequiresReturn)
{
    // Converte a entidade do banco no DTO. Um lugar só faz essa conversão,
    // então o controller nunca monta o DTO à mão.
    public static StockCategoryDto FromEntity(StockCategory c) => new(
        c.Id,
        c.Name,
        c.TracksProjectBalance,
        c.RequiresReturn);
}

// O que o front envia ao criar.
public sealed record CreateStockCategoryRequest(
    string Name,
    bool TracksProjectBalance,
    bool RequiresReturn);

// O que o front envia ao editar (hoje igual ao de criar, mas separado para
// poderem evoluir de forma independente).
public sealed record UpdateStockCategoryRequest(
    string Name,
    bool TracksProjectBalance,
    bool RequiresReturn);