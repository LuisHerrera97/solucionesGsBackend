using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IBotonRepositorio
    {
        Task<IEnumerable<Boton>> GetAllAsync(int? page = null, int? pageSize = null);
        Task<Boton> GetByIdAsync(Guid id);
        Task<Boton> GetByClaveAsync(string clave);
        Task<IEnumerable<Boton>> GetByPaginaIdAsync(Guid paginaId);
        Task<Boton> AddAsync(Boton boton);
        Task UpdateAsync(Boton boton);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
