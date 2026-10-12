using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;

namespace NoPrumo.Controllers;

[ApiController]
[Route("api/purchase-requests")]
[Authorize]
public sealed class PurchaseRequestsController(IPurchaseRequestService purchaseRequestService) : ControllerBase
{
    private bool CanManagePurchases => User.HasClaim("permission", "manage_purchases");

    private long? CurrentUserId
    {
        get
        {
            var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return long.TryParse(subject, out var id) ? id : null;
        }
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<PurchaseRequestDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] long? projectId = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        if (!User.HasClaim("permission", "manage_stock") && !CanManagePurchases && !User.HasClaim("permission", "manage_projects"))
        {
            return Forbid();
        }

        var result = await purchaseRequestService.ListAsync(page, size, projectId, status, CanManagePurchases, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PurchaseRequestDto>> GetById(long id, CancellationToken cancellationToken)
    {
        if (!User.HasClaim("permission", "manage_stock") && !CanManagePurchases && !User.HasClaim("permission", "manage_projects"))
        {
            return Forbid();
        }

        var result = await purchaseRequestService.GetByIdAsync(id, CanManagePurchases, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "manage_stock")]
    public async Task<ActionResult<PurchaseRequestDto>> Create(
        CreatePurchaseRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await purchaseRequestService.CreateAsync(request, CurrentUserId, CanManagePurchases, cancellationToken);
        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Request!.Id }, result.Request);
    }

    [HttpPost("{id:long}/decide")]
    [Authorize(Policy = "manage_purchases")]
    public async Task<ActionResult<PurchaseRequestDto>> Decide(
        long id,
        DecidePurchaseRequestRequest request,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result = await purchaseRequestService.DecideAsync(id, request, userId.Value, CanManagePurchases, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return Ok(result.Request);
    }

    [HttpPost("{id:long}/purchase")]
    [Authorize(Policy = "manage_purchases")]
    public async Task<ActionResult<PurchaseRequestDto>> RegisterPurchase(
        long id,
        RegisterPurchaseRequest request,
        CancellationToken cancellationToken)
    {
        var userId = CurrentUserId;
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var result = await purchaseRequestService.RegisterPurchaseAsync(id, request, userId.Value, CanManagePurchases, cancellationToken);
        if (result is null)
        {
            return NotFound();
        }

        if (!result.Succeeded)
        {
            return ValidationProblemFrom(result.Errors);
        }

        return Ok(result.Request);
    }

    private ActionResult ValidationProblemFrom(IReadOnlyDictionary<string, string> errors)
    {
        foreach (var (field, message) in errors)
        {
            ModelState.AddModelError(field, message);
        }

        return ValidationProblem(ModelState);
    }
}
