using NoPrumo.Application.DTOs;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;

namespace NoPrumo.Application.Services;

public sealed class PurchaseRequestService(
    IPurchaseRequestRepository purchaseRequestRepository,
    IProjectRepository projectRepository,
    TimeProvider timeProvider) : IPurchaseRequestService
{
    private const int MaxPageSize = 100;

    public async Task<PagedResult<PurchaseRequestDto>> ListAsync(
        int page,
        int size,
        long? projectId,
        string? status,
        bool canManagePurchases,
        CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        PurchaseRequestStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<PurchaseRequestStatus>(status, true, out var statusEnum))
        {
            parsedStatus = statusEnum;
        }

        var pagedResult = await purchaseRequestRepository.ListAsync(page, size, projectId, parsedStatus, cancellationToken);

        var items = pagedResult.Items
            .Select(request => ToDto(request, canManagePurchases))
            .ToList();

        return new PagedResult<PurchaseRequestDto>(
            items,
            pagedResult.Page,
            pagedResult.Size,
            pagedResult.Total,
            pagedResult.TotalPages);
    }

    public async Task<PurchaseRequestDto?> GetByIdAsync(
        long id,
        bool canManagePurchases,
        CancellationToken cancellationToken)
    {
        var request = await purchaseRequestRepository.GetByIdAsync(id, cancellationToken);
        return request is null ? null : ToDto(request, canManagePurchases);
    }

    public async Task<SavePurchaseRequestResult> CreateAsync(
        CreatePurchaseRequestRequest request,
        long? requestedByUserId,
        bool canManagePurchases,
        CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string>();

        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project is null)
        {
            errors["projectId"] = "Project does not exist.";
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            errors["items"] = "Purchase request must contain at least one item.";
        }
        else
        {
            for (var index = 0; index < request.Items.Count; index++)
            {
                var item = request.Items[index];
                if (item.RequestedQuantity <= 0)
                {
                    errors[$"items[{index}].requestedQuantity"] = "Requested quantity must be greater than zero.";
                }

                if (string.IsNullOrWhiteSpace(item.Unit))
                {
                    errors[$"items[{index}].unit"] = "Unit is required.";
                }
            }
        }

        if (errors.Count > 0)
        {
            return new SavePurchaseRequestResult(null, errors);
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;
        var today = timeProvider.Today();

        var purchaseRequest = new PurchaseRequest
        {
            ProjectId = request.ProjectId,
            RequestedBy = requestedByUserId,
            Date = today,
            NeededByDate = request.NeededByDate,
            Status = PurchaseRequestStatus.Pending,
            Notes = request.Notes?.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            Items = request.Items!.Select(item => new PurchaseRequestItem
            {
                StockItemId = item.StockItemId,
                RequestedQuantity = item.RequestedQuantity,
                FulfilledQuantity = 0m,
                Unit = item.Unit.Trim(),
                Notes = item.Notes?.Trim(),
            }).ToList()
        };

        purchaseRequestRepository.Add(purchaseRequest);
        await purchaseRequestRepository.SaveChangesAsync(cancellationToken);

        var created = await GetByIdAsync(purchaseRequest.Id, canManagePurchases, cancellationToken);
        return new SavePurchaseRequestResult(created, errors);
    }

    public async Task<SavePurchaseRequestResult?> DecideAsync(
        long id,
        DecidePurchaseRequestRequest request,
        long decidedByUserId,
        bool canManagePurchases,
        CancellationToken cancellationToken)
    {
        var purchaseRequest = await purchaseRequestRepository.GetForUpdateAsync(id, cancellationToken);
        if (purchaseRequest is null)
        {
            return null;
        }

        var errors = new Dictionary<string, string>();
        var now = timeProvider.GetUtcNow().UtcDateTime;

        if (request.Approved)
        {
            if (!purchaseRequest.Approve(decidedByUserId, now))
            {
                errors["status"] = "Only pending purchase requests can be approved.";
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                errors["rejectionReason"] = "A reason is required when rejecting a purchase request.";
            }
            else if (!purchaseRequest.Reject(decidedByUserId, now, request.RejectionReason.Trim()))
            {
                errors["status"] = "Only pending purchase requests can be rejected.";
            }
        }

        if (errors.Count > 0)
        {
            return new SavePurchaseRequestResult(null, errors);
        }

        await purchaseRequestRepository.SaveChangesAsync(cancellationToken);

        var updated = await GetByIdAsync(id, canManagePurchases, cancellationToken);
        return new SavePurchaseRequestResult(updated, errors);
    }

    public async Task<SavePurchaseRequestResult?> RegisterPurchaseAsync(
        long id,
        RegisterPurchaseRequest request,
        long recordedByUserId,
        bool canManagePurchases,
        CancellationToken cancellationToken)
    {
        var purchaseRequest = await purchaseRequestRepository.GetForUpdateAsync(id, cancellationToken);
        if (purchaseRequest is null)
        {
            return null;
        }

        var errors = new Dictionary<string, string>();
        if (purchaseRequest.Status != PurchaseRequestStatus.Approved)
        {
            errors["status"] = "Only approved purchase requests can have purchases registered.";
            return new SavePurchaseRequestResult(null, errors);
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            errors["items"] = "At least one purchase item is required.";
            return new SavePurchaseRequestResult(null, errors);
        }

        var now = timeProvider.GetUtcNow().UtcDateTime;

        foreach (var purchaseItem in request.Items)
        {
            var item = purchaseRequest.Items.FirstOrDefault(i => i.Id == purchaseItem.PurchaseRequestItemId);
            if (item is null)
            {
                errors["items"] = $"Item with ID {purchaseItem.PurchaseRequestItemId} was not found in this request.";
                return new SavePurchaseRequestResult(null, errors);
            }

            if (purchaseItem.Quantity <= 0)
            {
                errors["quantity"] = "Quantity must be greater than zero.";
                return new SavePurchaseRequestResult(null, errors);
            }

            item.Fulfill(purchaseItem.Quantity);

            var movement = new StockMovement
            {
                StockItemId = item.StockItemId,
                ProjectId = purchaseRequest.ProjectId,
                Quantity = purchaseItem.Quantity,
                Unit = item.Unit,
                UnitCost = purchaseItem.UnitCost,
                SupplierId = purchaseItem.SupplierId ?? request.SupplierId,
                InvoiceNumber = purchaseItem.InvoiceNumber ?? request.InvoiceNumber,
                Type = "purchase",
                SourceType = "purchase_request",
                SourceId = purchaseRequest.Id,
                Date = now,
                RecordedBy = recordedByUserId,
                CreatedAt = now
            };

            purchaseRequestRepository.AddMovement(movement);
        }

        var allFulfilled = purchaseRequest.Items.All(i => i.FulfilledQuantity >= i.RequestedQuantity);
        if (allFulfilled)
        {
            purchaseRequest.MarkPurchased(now);
        }

        await purchaseRequestRepository.SaveChangesAsync(cancellationToken);

        var updated = await GetByIdAsync(id, canManagePurchases, cancellationToken);
        return new SavePurchaseRequestResult(updated, errors);
    }

    private static PurchaseRequestDto ToDto(PurchaseRequest request, bool canManagePurchases)
    {
        var items = request.Items
            .Select(item => new PurchaseRequestItemDto
            {
                Id = item.Id,
                StockItemId = item.StockItemId,
                StockItemName = item.StockItem?.Name ?? "Item",
                RequestedQuantity = item.RequestedQuantity,
                FulfilledQuantity = item.FulfilledQuantity,
                Unit = item.Unit,
                Notes = item.Notes
            })
            .ToList();

        var baseDto = new PurchaseRequestDto
        {
            Id = request.Id,
            ProjectId = request.ProjectId,
            ProjectName = request.Project?.Name ?? "Obra",
            RequestedBy = request.RequestedBy,
            RequesterName = request.RequestedByUser?.Name,
            Date = request.Date,
            NeededByDate = request.NeededByDate,
            Status = request.Status.ToString(),
            Notes = request.Notes,
            DecidedBy = request.DecidedBy,
            DeciderName = request.DecidedByUser?.Name,
            DecidedAt = request.DecidedAt,
            RejectionReason = request.RejectionReason,
            CreatedAt = request.CreatedAt,
            Items = items
        };

        return canManagePurchases
            ? new PurchaseRequestPurchasingDto(baseDto, true)
            : baseDto;
    }
}
