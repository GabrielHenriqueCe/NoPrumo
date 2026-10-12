namespace NoPrumo.Domain.Enums;

// Issued vem primeiro de propósito: a coluna tem DEFAULT 'issued', e o EF não
// manda o primeiro valor do enum, deixa o banco preencher.
public enum PpeItemStatus
{
    Issued,
    Returned,
    Lost
}
