namespace NoPrumo.Application.DTOs;

// Sem preço: a ficha é comprovante de entrega, não custo.
public sealed record PpeIssueDto
{
    public required long Id { get; init; }
    public required long EmployeeId { get; init; }
    public required string EmployeeName { get; init; }
    public required string? RegistrationNumber { get; init; }
    public required long? ProjectId { get; init; }
    public required string? ProjectName { get; init; }
    public required DateOnly Date { get; init; }
    public required string? Notes { get; init; }
    public required string? IssuedByName { get; init; }
    public required bool Signed { get; init; }
    public required DateTime? SignedAt { get; init; }
    public required IReadOnlyList<PpeIssueItemDto> Items { get; init; }
}
