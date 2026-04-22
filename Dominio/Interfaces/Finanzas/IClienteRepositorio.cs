using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas;

namespace FinancieraSoluciones.Domain.Interfaces.Finanzas
{
    public interface IClienteRepositorio
    {
        Task<(IEnumerable<Cliente> Items, int TotalCount)> GetAllAsync(int? page = null, int? pageSize = null, string? buscar = null, Guid? zonaId = null, bool aplicarFiltroZona = false);
        Task<Cliente> GetByIdAsync(Guid id);
        Task<Cliente> AddAsync(Cliente cliente);
        Task UpdateAsync(Cliente cliente);
        Task DeleteAsync(Guid id);
    }
}
