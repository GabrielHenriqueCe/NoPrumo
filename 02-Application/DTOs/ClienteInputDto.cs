namespace NoPrumo.Application.DTOs
{
    public record ClienteInputDto(
        string Nome,
        int ObrasSimultaneas,
        decimal ValorOrcamento);
}
