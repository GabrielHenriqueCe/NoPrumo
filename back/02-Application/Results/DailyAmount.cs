namespace NoPrumo.Application.Results;

// Soma de um dia, como o repositório devolve; o service junta material e mão de obra.
public sealed record DailyAmount(DateOnly Date, decimal Amount);
