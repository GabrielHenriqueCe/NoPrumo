using NoPrumo.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoPrumo.Application.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<ClienteOutputDto>> GetAllAsync();
        Task<ClienteOutputDto> GetByIdAsync(int id);
        Task<ClienteOutputDto> CreateAsync(ClienteInputDto dto);
        Task UpdateAsync(int id, ClienteInputDto dto);
        Task DeleteAsync(int id);
    }
}
