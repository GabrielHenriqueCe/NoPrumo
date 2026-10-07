namespace NoPrumo.Application.Requests;

// Texto chega anulável para a validação ser do controller, com a mensagem no
// campo certo — senão o [ApiController] responde antes, com o texto do .NET.
public sealed record SaveTrainingTypeRequest
{
    public string? Code { get; init; }
    public string? Name { get; init; }
    public int? ValidityMonths { get; init; }
    public int? MinWorkloadHours { get; init; }
    public bool RequiresInPerson { get; init; }
}
