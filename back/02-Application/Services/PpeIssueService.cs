using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using NoPrumo.Application.DTOs;
using NoPrumo.Application.Extensions;
using NoPrumo.Application.Interfaces;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;
using NoPrumo.Domain.Entities;

namespace NoPrumo.Application.Services;

public sealed class PpeIssueService(IPpeIssueRepository ppeIssueRepository, TimeProvider timeProvider) : IPpeIssueService
{
    private const int MaxPageSize = 100;
    private const int MaxNotesLength = 2000;

    public async Task<PagedResult<PpeIssueDto>> ListAsync(int page, int size, string? search, long? projectId, long? employeeId, CancellationToken cancellationToken)
    {
        page = Math.Max(1, page);
        size = Math.Clamp(size, 1, MaxPageSize);

        var issuePage = await ppeIssueRepository.ListAsync(page, size, search, projectId, employeeId, cancellationToken);
        var items = issuePage.Items.Select(ToDto).ToList();

        return new PagedResult<PpeIssueDto>(items, issuePage.Page, issuePage.Size, issuePage.Total, issuePage.TotalPages);
    }

    public async Task<PpeIssueOptionsDto> GetOptionsAsync(CancellationToken cancellationToken)
    {
        var employees = await ppeIssueRepository.ListActiveEmployeesAsync(cancellationToken);
        var stockItems = await ppeIssueRepository.ListActiveStockItemsAsync(cancellationToken);
        var today = timeProvider.Today();

        return new PpeIssueOptionsDto
        {
            Employees = employees
                .Select(employee => new EmployeeOptionDto { Id = employee.Id, Name = employee.Name, RegistrationNumber = employee.RegistrationNumber })
                .ToList(),
            Items = stockItems
                .Where(stockItem => stockItem.StockGroup.StockCategory.IsPpe())
                .Select(stockItem => new PpeStockItemOptionDto
                {
                    Id = stockItem.Id,
                    Name = stockItem.Name,
                    Unit = stockItem.Unit,
                    Ca = stockItem.Ca,
                    CaExpiryDate = stockItem.CaExpiryDate,
                    CaExpired = stockItem.IsCaExpiredOn(today),
                })
                .ToList(),
        };
    }

    public async Task<PpeIssueDto?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var issue = await ppeIssueRepository.GetByIdAsync(id, cancellationToken);
        return issue is null ? null : ToDto(issue);
    }

    public async Task<SavePpeIssueResult> CreateAsync(SavePpeIssueRequest request, long? issuedById, CancellationToken cancellationToken)
    {
        var today = timeProvider.Today();
        var errors = new Dictionary<string, string>();

        var employee = request.EmployeeId is null
            ? null
            : await ppeIssueRepository.GetEmployeeAsync(request.EmployeeId.Value, cancellationToken);

        if (employee is null)
        {
            errors["employeeId"] = "Pick an employee.";
        }
        else if (employee.Active != true)
        {
            errors["employeeId"] = "This employee is inactive.";
        }

        if (request.ProjectId is { } projectId && !await ppeIssueRepository.ActiveProjectExistsAsync(projectId, cancellationToken))
        {
            errors["projectId"] = "Pick an active project.";
        }

        if (request.Date is null)
        {
            errors["date"] = "Date is required.";
        }
        else if (request.Date > today)
        {
            errors["date"] = "The date cannot be in the future.";
        }

        if (request.Notes?.Trim().Length > MaxNotesLength)
        {
            errors["notes"] = $"Use at most {MaxNotesLength} characters.";
        }

        var lines = request.Items ?? [];
        var itemProblems = new List<string>();

        if (lines.Count == 0)
        {
            itemProblems.Add("Add at least one item.");
        }

        if (lines.Any(line => line.StockItemId is null))
        {
            itemProblems.Add("Pick the item on every line.");
        }

        if (lines.Any(line => line.Quantity is null or <= 0))
        {
            itemProblems.Add("Every quantity must be greater than zero.");
        }

        var requestedIds = lines.Where(line => line.StockItemId is not null).Select(line => line.StockItemId!.Value).ToList();
        if (requestedIds.Count != requestedIds.Distinct().Count())
        {
            itemProblems.Add("Each item can appear only once; add up the quantity instead.");
        }

        var stockItems = await ppeIssueRepository.ListStockItemsAsync(requestedIds.Distinct().ToList(), cancellationToken);

        foreach (var stockItemId in requestedIds.Distinct())
        {
            var stockItem = stockItems.FirstOrDefault(candidate => candidate.Id == stockItemId);

            if (stockItem is null || stockItem.Active != true)
            {
                itemProblems.Add("One of the items does not exist or is inactive.");
            }
            else if (!stockItem.StockGroup.StockCategory.IsPpe())
            {
                itemProblems.Add($"{stockItem.Name} is not PPE.");
            }
            else if (stockItem.IsCaExpiredOn(today))
            {
                itemProblems.Add($"{stockItem.Name}: CA expired on {Format(stockItem.CaExpiryDate!.Value)}. It cannot be issued.");
            }
        }

        if (itemProblems.Count > 0)
        {
            errors["items"] = string.Join(" ", itemProblems.Distinct());
        }

        if (errors.Count > 0)
        {
            return new SavePpeIssueResult(null, errors);
        }

        var date = request.Date!.Value;
        var issue = new PpeIssue
        {
            EmployeeId = employee!.Id,
            ProjectId = request.ProjectId,
            Date = date,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            IssuedById = issuedById,
        };

        foreach (var line in lines)
        {
            var stockItem = stockItems.First(candidate => candidate.Id == line.StockItemId);
            issue.Items.Add(stockItem.IssueAsPpe(line.Quantity!.Value, date));
        }

        await ppeIssueRepository.AddWithStockExitsAsync(issue, timeProvider.Now(), issuedById, cancellationToken);

        return new SavePpeIssueResult(await GetByIdAsync(issue.Id, cancellationToken), errors);
    }

    public async Task<SavePpeIssueResult?> SignAsync(long id, SignPpeIssueRequest request, CancellationToken cancellationToken)
    {
        var issue = await ppeIssueRepository.GetForUpdateAsync(id, cancellationToken);
        if (issue is null)
        {
            return null;
        }

        var errors = new Dictionary<string, string>();
        var typedName = request.TypedName?.Trim() ?? string.Empty;

        if (issue.IsSigned())
        {
            errors["typedName"] = "This delivery is already signed.";
        }
        else if (typedName.Length == 0)
        {
            errors["typedName"] = "The employee has to type their name.";
        }
        else if (!issue.Employee.IsNamed(typedName))
        {
            errors["typedName"] = "The name does not match the employee's registered name.";
        }

        if (errors.Count > 0)
        {
            return new SavePpeIssueResult(null, errors);
        }

        var signedAt = timeProvider.Now();
        issue.Sign(signedAt, SignatureHash(issue, signedAt));
        await ppeIssueRepository.SaveChangesAsync(cancellationToken);

        return new SavePpeIssueResult(await GetByIdAsync(id, cancellationToken), errors);
    }

    public async Task<SavePpeIssueResult?> ReturnItemAsync(long id, long itemId, ReturnPpeItemRequest request, CancellationToken cancellationToken)
    {
        var issue = await ppeIssueRepository.GetForUpdateAsync(id, cancellationToken);
        var item = issue?.Items.FirstOrDefault(candidate => candidate.Id == itemId);
        if (item is null)
        {
            return null;
        }

        var errors = new Dictionary<string, string>();

        if (!item.IsIssued())
        {
            errors["returnDate"] = "This item is no longer with the employee.";
        }
        else if (request.ReturnDate is null)
        {
            errors["returnDate"] = "Return date is required.";
        }
        else if (item.ReturnsBeforeIssue(request.ReturnDate.Value))
        {
            errors["returnDate"] = $"The return cannot be before the issue ({Format(item.IssueDate!.Value)}).";
        }

        if (errors.Count > 0)
        {
            return new SavePpeIssueResult(null, errors);
        }

        item.Return(request.ReturnDate!.Value);
        await ppeIssueRepository.SaveChangesAsync(cancellationToken);

        return new SavePpeIssueResult(await GetByIdAsync(id, cancellationToken), errors);
    }

    public async Task<SavePpeIssueResult?> MarkItemLostAsync(long id, long itemId, CancellationToken cancellationToken)
    {
        var issue = await ppeIssueRepository.GetForUpdateAsync(id, cancellationToken);
        var item = issue?.Items.FirstOrDefault(candidate => candidate.Id == itemId);
        if (item is null)
        {
            return null;
        }

        if (!item.IsIssued())
        {
            return new SavePpeIssueResult(null, new Dictionary<string, string> { ["status"] = "This item is no longer with the employee." });
        }

        item.MarkLost();
        await ppeIssueRepository.SaveChangesAsync(cancellationToken);

        return new SavePpeIssueResult(await GetByIdAsync(id, cancellationToken), new Dictionary<string, string>());
    }

    // SHA-256 de funcionário + entrega + data da assinatura, em hexadecimal: os 64
    // caracteres de signature_hash.
    private static string SignatureHash(PpeIssue issue, DateTime signedAt)
    {
        var content = $"{issue.EmployeeId}|{issue.Id}|{signedAt.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture)}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content))).ToLowerInvariant();
    }

    private static PpeIssueDto ToDto(PpeIssue issue) => new()
    {
        Id = issue.Id,
        EmployeeId = issue.EmployeeId,
        EmployeeName = issue.Employee.Name,
        RegistrationNumber = issue.Employee.RegistrationNumber,
        ProjectId = issue.ProjectId,
        ProjectName = issue.Project?.Name,
        Date = issue.Date,
        Notes = issue.Notes,
        IssuedByName = issue.IssuedBy?.Name,
        Signed = issue.IsSigned(),
        SignedAt = issue.SignedAt,
        Items = issue.Items
            .OrderBy(item => item.Id)
            .Select(item => new PpeIssueItemDto
            {
                Id = item.Id,
                StockItemId = item.StockItemId,
                ItemName = item.ItemNameSnapshot,
                Ca = item.CaSnapshot,
                Quantity = item.Quantity,
                Unit = item.Unit,
                Status = item.Status,
                IssueDate = item.IssueDate,
                ReturnDate = item.ReturnDate,
            })
            .ToList(),
    };

    private static string Format(DateOnly date) =>
        date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
}
