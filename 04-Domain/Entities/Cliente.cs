using System.ComponentModel.DataAnnotations;


namespace NoPrumo.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; private set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; private set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade de obras não pode ser negativa")]
        public int ObrasSimultaneas { get; private set; }

        [Range(0, double.MaxValue, ErrorMessage = "O orçamento deve ser maior que zero")]
        public decimal ValorOrcamento { get; private set; }

        private Cliente() { }

        public Cliente(string nome, int obrasSimultaneas, decimal valorOrcamento)
        {
            Nome = nome;
            ObrasSimultaneas = obrasSimultaneas;
            ValorOrcamento = valorOrcamento;
        }

        public void Atualizar(string nome, int obrasSimultaneas, decimal valorOrcamento)
        {
            Nome = nome;
            ObrasSimultaneas = obrasSimultaneas;
            ValorOrcamento = valorOrcamento;
        }
    }
}
