using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IPermisoBotonRepositorio
    {
        Task<IEnumerable<PermisoBoton>> GetAllAsync();
        Task<PermisoBoton> GetByIdAsync(Guid id);
        Task<IEnumerable<PermisoBoton>> GetByPerfilIdAsync(Guid perfilId);
        Task<PermisoBoton> GetByPerfilAndBotonAsync(Guid perfilId, Guid botonId);
        Task<bool> HasPermisoAsync(Guid perfilId, string botonClave);
        Task<bool> HasPermisoAlgunoAsync(Guid perfilId, params string[] botonClaves);
        Task<PermisoBoton> AddAsync(PermisoBoton permisoBoton);
        Task UpdateAsync(PermisoBoton permisoBoton);
        Task DeleteAsync(Guid id);
        Task DeleteByPerfilIdAsync(Guid perfilId);
        Task<bool> ExistsAsync(Guid id);
    }
}
