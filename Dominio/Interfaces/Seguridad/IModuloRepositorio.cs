using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IModuloRepositorio
    {
        Task<IEnumerable<Modulo>> GetAllAsync(int? page = null, int? pageSize = null);
        Task<Modulo> GetByIdAsync(Guid id);
        Task<Modulo> GetByClaveAsync(string clave);
        Task<Modulo> AddAsync(Modulo modulo);
        Task UpdateAsync(Modulo modulo);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
