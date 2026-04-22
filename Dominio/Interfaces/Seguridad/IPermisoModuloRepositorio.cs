using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IPermisoModuloRepositorio
    {
        Task<IEnumerable<PermisoModulo>> GetAllAsync();
        Task<PermisoModulo> GetByIdAsync(Guid id);
        Task<IEnumerable<PermisoModulo>> GetByPerfilIdAsync(Guid perfilId);
        Task<PermisoModulo> GetByPerfilAndModuloAsync(Guid perfilId, Guid moduloId);
        Task<PermisoModulo> AddAsync(PermisoModulo permisoModulo);
        Task UpdateAsync(PermisoModulo permisoModulo);
        Task DeleteAsync(Guid id);
        Task DeleteByPerfilIdAsync(Guid perfilId);
        Task<bool> ExistsAsync(Guid id);
    }
}