using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Cortes;

namespace FinancieraSoluciones.Domain.Interfaces.Finanzas.Cortes
{
    public interface ICorteCajaRepositorio
    {
        Task<CorteCaja> AddAsync(CorteCaja corte);
        Task<string> ObtenerSiguienteFolioAsync(DateTime fecha);
        Task<IEnumerable<CorteCaja>> ObtenerEnRangoAsync(DateTime fechaInicio, DateTime fechaFin, int? page = null, int? pageSize = null);
    }
}
