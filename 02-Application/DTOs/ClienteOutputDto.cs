namespace NoPrumo.Application.DTOs
{
    public record ClienteOutputDto(
        int Id,
        string Nome,
        int ObrasSimultaneas,
        decimal ValorOrcamento);
}
