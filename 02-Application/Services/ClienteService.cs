using NoPrumo.Application.DTOs;
using NoPrumo.Application.Interfaces;
using NoPrumo.Domain.Entities;
using NoPrumo.Domain.Interfaces;

namespace NoPrumo.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _repository;

        public ClienteService(IClienteRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ClienteOutputDto>> GetAllAsync()
        {
            var clientes = await _repository.GetAllAsync();
            return clientes.Select(ToOutput);
        }

        public async Task<ClienteOutputDto> GetByIdAsync(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
                throw new KeyNotFoundException($"Cliente {id} não encontrado.");

            return ToOutput(cliente);
        }

        public async Task<ClienteOutputDto> CreateAsync(ClienteInputDto dto)
        {
            var cliente = new Cliente(dto.Nome, dto.ObrasSimultaneas, dto.ValorOrcamento);

            await _repository.AddAsync(cliente);
            await _repository.SaveChangesAsync();

            return ToOutput(cliente);
        }

        public async Task UpdateAsync(int id, ClienteInputDto dto)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
                throw new KeyNotFoundException($"Cliente {id} não encontrado.");

            cliente.Atualizar(dto.Nome, dto.ObrasSimultaneas, dto.ValorOrcamento);

            _repository.Update(cliente);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cliente = await _repository.GetByIdAsync(id);
            if (cliente == null)
                throw new KeyNotFoundException($"Cliente {id} não encontrado.");

            _repository.Delete(cliente);
            await _repository.SaveChangesAsync();
        }

        private static ClienteOutputDto ToOutput(Cliente cliente) =>
            new(cliente.Id, cliente.Nome, cliente.ObrasSimultaneas, cliente.ValorOrcamento);
    }
}