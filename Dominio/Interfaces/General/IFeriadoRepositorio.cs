using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.General;

namespace FinancieraSoluciones.Domain.Interfaces.General
{
    public interface IFeriadoRepositorio
    {
        Task<IEnumerable<Feriado>> GetAllAsync();
        Task<IEnumerable<Feriado>> GetActivosEnRangoAsync(DateTime desde, DateTime hasta);
        Task<Feriado> AddAsync(Feriado feriado);
        Task<Feriado> GetByIdAsync(Guid id);
        Task<Feriado> GetByFechaAsync(DateTime fecha);
        Task UpdateAsync(Feriado feriado);
        Task DeleteAsync(Guid id);
    }
}
