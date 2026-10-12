using NoPrumo.Application.DTOs;
using NoPrumo.Application.Requests;
using NoPrumo.Application.Results;

namespace NoPrumo.Application.Interfaces;

public interface IPpeIssueService
{
    Task<PagedResult<PpeIssueDto>> ListAsync(int page, int size, string? search, long? projectId, long? employeeId, CancellationToken cancellationToken);

    Task<PpeIssueOptionsDto> GetOptionsAsync(CancellationToken cancellationToken);

    Task<PpeIssueDto?> GetByIdAsync(long id, CancellationToken cancellationToken);

    Task<SavePpeIssueResult> CreateAsync(SavePpeIssueRequest request, long? issuedById, CancellationToken cancellationToken);

    Task<SavePpeIssueResult?> SignAsync(long id, SignPpeIssueRequest request, CancellationToken cancellationToken);

    Task<SavePpeIssueResult?> ReturnItemAsync(long id, long itemId, ReturnPpeItemRequest request, CancellationToken cancellationToken);

    Task<SavePpeIssueResult?> MarkItemLostAsync(long id, long itemId, CancellationToken cancellationToken);
}
