using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.General;

namespace FinancieraSoluciones.Domain.Interfaces.General
{
    public interface IZonaCobranzaRepositorio
    {
        Task<IEnumerable<ZonaCobranza>> GetAllAsync(int? page = null, int? pageSize = null);
        Task<ZonaCobranza> GetByIdAsync(Guid id);
        Task<ZonaCobranza> AddAsync(ZonaCobranza zona);
        Task UpdateAsync(ZonaCobranza zona);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsByNombreAsync(string nombre);
    }
}
