using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IPermisoPaginaRepositorio
    {
        Task<IEnumerable<PermisoPagina>> GetAllAsync();
        Task<PermisoPagina> GetByIdAsync(Guid id);
        Task<IEnumerable<PermisoPagina>> GetByPerfilIdAsync(Guid perfilId);
        Task<PermisoPagina> GetByPerfilAndPaginaAsync(Guid perfilId, Guid paginaId);
        Task<PermisoPagina> AddAsync(PermisoPagina permisoPagina);
        Task UpdateAsync(PermisoPagina permisoPagina);
        Task DeleteAsync(Guid id);
        Task DeleteByPerfilIdAsync(Guid perfilId);
        Task DeleteByPaginaIdAsync(Guid paginaId);
        Task<bool> ExistsAsync(Guid id);
    }
}