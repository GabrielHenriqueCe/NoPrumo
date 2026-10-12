using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;

namespace NoPrumo.Controllers;

/// <summary>
/// Entrada e saída de estoque. Não há edição nem exclusão: um lançamento
/// errado se corrige com outro lançamento de sinal oposto, nunca alterando o
/// que já aconteceu.
///
/// O saldo nunca é lido de uma coluna — é sempre a soma das movimentações.
/// Para categorias com saldo por obra (consumable material), a soma é
/// filtrada por obra; para as demais (tool), é do item inteiro.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "view_stock")]
public sealed class StockMovementsController(IStockMovementService stockMovementService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<StockMovementDto>>> List(
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] long? stockItemId = null,
        [FromQuery] long? projectId = null)
    {
        var movementPage = await stockMovementService.ListAsync(page, size, stockItemId, projectId, cancellationToken);

        return Ok(movementPage);
    }

    [HttpGet("balance")]
    public async Task<ActionResult<StockBalanceDto>> GetBalance(
        [FromQuery] long stockItemId,
        CancellationToken cancellationToken,
        [FromQuery] long? projectId = null)
    {
        var balance = await stockMovementService.GetBalanceAsync(stockItemId, projectId, cancellationToken);

        // null = item não existe; só o controller sabe que isso vira 404
        if (balance is null) return NotFound();

        return Ok(balance);
    }

    [HttpGet("below-minimum")]
    public async Task<ActionResult<IReadOnlyList<StockBelowMinimumDto>>> ListBelowMinimum(CancellationToken cancellationToken)
    {
        var belowMinimum = await stockMovementService.ListBelowMinimumAsync(cancellationToken);

        return Ok(belowMinimum);
    }

    [HttpPost]
    [Authorize(Policy = "manage_stock")]
    public async Task<ActionResult<StockMovementDto>> Create(
        CreateStockMovementRequest request,
        CancellationToken cancellationToken)
    {
        var result = await stockMovementService.CreateAsync(request, cancellationToken);

        // Bilhete de erros do service vira 400 com os campos
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return ValidationProblem(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, result.Movement);
    }
}