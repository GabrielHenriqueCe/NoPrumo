using NoPrumo.Domain.Entities;

namespace NoPrumo.Domain.Interfaces
{
    public interface IClienteRepository
    {
        Task<IEnumerable<Cliente>> GetAllAsync();
        Task<Cliente?> GetByIdAsync(int id);
        Task AddAsync(Cliente cliente);
        void Update(Cliente cliente);
        void Delete(Cliente cliente);
        Task SaveChangesAsync();
    }
}
