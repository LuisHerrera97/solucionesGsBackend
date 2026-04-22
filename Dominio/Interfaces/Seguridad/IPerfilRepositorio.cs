using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IPerfilRepositorio
    {
        Task<IEnumerable<Perfil>> GetAllAsync(int? page = null, int? pageSize = null);
        Task<Perfil> GetByIdAsync(Guid id);
        Task<Perfil> GetByClaveAsync(string clave);
        Task<Perfil> AddAsync(Perfil perfil);
        Task UpdateAsync(Perfil perfil);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
