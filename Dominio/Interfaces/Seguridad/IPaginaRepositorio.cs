using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IPaginaRepositorio
    {
        Task<IEnumerable<Pagina>> GetAllAsync(int? page = null, int? pageSize = null);
        Task<Pagina> GetByIdAsync(Guid id);
        Task<Pagina> GetByClaveAsync(string clave);
        Task<IEnumerable<Pagina>> GetByModuloIdAsync(Guid moduloId);
        Task<Pagina> AddAsync(Pagina pagina);
        Task UpdateAsync(Pagina pagina);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
