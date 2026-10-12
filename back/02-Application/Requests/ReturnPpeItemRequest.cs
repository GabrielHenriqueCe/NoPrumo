namespace NoPrumo.Application.Requests;

public sealed record ReturnPpeItemRequest
{
    public DateOnly? ReturnDate { get; init; }
}
