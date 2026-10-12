using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Enums;
using NoPrumo.Application.DTOs;

namespace NoPrumo.Application.Services;


public sealed class StockMovementService(
    IStockMovementRepository stockMovementRepository,
    TimeProvider timeProvider) : IStockMovementService
{
    private const int MaxPageSize = 100;

    public async Task<SaveStockMovementResult> CreateAsync(
    CreateStockMovementRequest request,
    CancellationToken cancellationToken)
    {
        // Bilhete de erros: chave = campo, valor = mensagem
        var errors = new Dictionary<string, string>();

        var item = await stockMovementRepository.GetStockItemWithCategoryAsync(request.StockItemId, cancellationToken);

        // Sem item não dá para conferir mais nada, então devolve já
        if (item is null)
        {
            errors["stockItemId"] = "Pick an item.";
            return new SaveStockMovementResult(null, errors);
        }

        if (item.Active != true)
        {
            errors["stockItemId"] = "This item is inactive.";
        }

        if (request.Quantity <= 0)
        {
            errors["quantity"] = "Quantity must be greater than zero.";
        }

        var category = item.StockGroup.StockCategory;

        // Nem por obra nem com devolução: é EPI, que tem fluxo próprio (PpeIssue)
        if (!category.TracksProjectBalance && !category.RequiresReturn)
        {
            errors["stockItemId"] = "This category does not use stock movements.";
            return new SaveStockMovementResult(null, errors);
        }

        Project? project = null;
        Employee? employee = null;

        if (category.TracksProjectBalance)
        {
            project = request.ProjectId is null
                ? null
                : await stockMovementRepository.GetProjectAsync(request.ProjectId.Value, cancellationToken);

            if (project is null)
            {
                errors["projectId"] = "Pick a project.";
            }
        }

        if (category.RequiresReturn)
        {
            employee = request.EmployeeId is null
                ? null
                : await stockMovementRepository.GetEmployeeAsync(request.EmployeeId.Value, cancellationToken);

            // Só é obrigatório na saída: é aí que a ferramenta fica com alguém
            if (request.Type == StockMovementType.Exit && employee is null)
            {
                errors["employeeId"] = "Pick an employee.";
            }
            else if (request.EmployeeId is not null && employee is null)
            {
                errors["employeeId"] = "Employee not found.";
            }
        }

        // Se algum campo falhou, devolve o bilhete de erros e não anota nada
        if (errors.Count > 0)
        {
            return new SaveStockMovementResult(null, errors);
        }

        // Na saída, não pode tirar mais do que existe (por obra ou global, conforme a categoria)
        if (request.Type == StockMovementType.Exit)
        {
            var balance = await stockMovementRepository.GetBalanceAsync(
                item.Id,
                category.TracksProjectBalance ? project!.Id : null,
                cancellationToken);

            if (request.Quantity > balance)
            {
                errors["quantity"] = "Not enough balance for this exit.";
                return new SaveStockMovementResult(null, errors);
            }
        }

        var movement = new StockMovement
        {
            StockItemId = item.Id,
            StockItem = item,
            Type = request.Type,
            Quantity = request.Quantity,
            Unit = item.Unit,
            Project = project,
            Employee = employee,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            Date = timeProvider.Now() // o "agora" vem de fora, não de DateTime.Now
        };

        await stockMovementRepository.AddAsync(movement, cancellationToken);

        return new SaveStockMovementResult(ToDto(movement), errors);
    }
    
    public async Task<PagedResult<StockMovementDto>> ListAsync(
        int page,
        int size,
        long? stockItemId,
        long? projectId,
        CancellationToken cancellationToken)
    {
        // Página mínima 1 e tamanho entre 1 e 100, como no controller
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var movementPage = await stockMovementRepository.ListAsync(page, size, stockItemId, projectId, cancellationToken);
        var items = movementPage.Items.Select(ToDto).ToList();

        return new PagedResult<StockMovementDto>(
            items,
            movementPage.Page,
            movementPage.Size,
            movementPage.Total,
            movementPage.TotalPages);
    }

    public async Task<StockBalanceDto?> GetBalanceAsync(
        long stockItemId,
        long? projectId,
        CancellationToken cancellationToken)
    {
        var item = await stockMovementRepository.GetStockItemWithCategoryAsync(stockItemId, cancellationToken);

        // Item não existe: devolve null e o controller responde 404
        if (item is null) return null;

        // Consumable material tem saldo por obra; ferramenta tem saldo global
        var scopeByProject = item.StockGroup.StockCategory.TracksProjectBalance;
        var effectiveProjectId = scopeByProject ? projectId : null;

        var balance = await stockMovementRepository.GetBalanceAsync(stockItemId, effectiveProjectId, cancellationToken);

        string? projectName = null;

        if (effectiveProjectId is not null)
        {
            var project = await stockMovementRepository.GetProjectAsync(effectiveProjectId.Value, cancellationToken);
            projectName = project?.Name;
        }

        return new StockBalanceDto(item.Id, item.Name, item.Unit, effectiveProjectId, projectName, balance);
    }

    public async Task<IReadOnlyList<StockBelowMinimumDto>> ListBelowMinimumAsync(CancellationToken cancellationToken)
    {
        // Duas consultas só: saldos somados por item e a lista de itens ativos.
        // O cruzamento é feito em memória, para não fazer uma query por item.
        var balancesByItem = await stockMovementRepository.ListBalancesByItemAsync(null, cancellationToken);
        var activeItems = await stockMovementRepository.ListActiveStockItemsAsync(cancellationToken);

        return activeItems
            .Select(item => new
            {
                item,
                balance = balancesByItem.GetValueOrDefault(item.Id, 0m),
            })
            .Where(entry => entry.balance < entry.item.MinQuantity)
            .Select(entry => new StockBelowMinimumDto(
                entry.item.Id,
                entry.item.Name,
                entry.item.Unit,
                entry.item.MinQuantity,
                entry.balance))
            .ToArray();
    }

    private static StockMovementDto ToDto(StockMovement movement) => new(
            movement.Id,
            movement.StockItemId,
            movement.StockItem.Name,
            movement.Type,
            movement.Quantity,
            movement.Unit,
            movement.ProjectId,
            movement.Project?.Name,
            movement.EmployeeId,
            movement.Employee?.Name,
            movement.Notes,
            movement.Date);

}