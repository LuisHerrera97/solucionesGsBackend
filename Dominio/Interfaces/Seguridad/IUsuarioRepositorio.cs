using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IUsuarioRepositorio
    {
        Task<IReadOnlyList<Usuario>> GetByIdsAsync(IReadOnlyCollection<Guid> ids);
        Task<IEnumerable<Usuario>> GetAllAsync(int? page = null, int? pageSize = null);
        Task<Usuario> GetByIdAsync(Guid id);
        Task<Usuario> GetByUsuarioAccesoAsync(string usuarioAcceso);
        Task<Usuario> AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByUsuarioAccesoAsync(string usuarioAcceso);
    }
}
