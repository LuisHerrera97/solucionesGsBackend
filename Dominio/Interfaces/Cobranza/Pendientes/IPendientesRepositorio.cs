using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Cobranza.Pendientes;

namespace FinancieraSoluciones.Domain.Interfaces.Cobranza.Pendientes
{
    public interface IPendientesRepositorio
    {
        Task<(IEnumerable<PendienteCobro> Items, int TotalCount)> ObtenerAsync(
            DateTime hoy,
            string? busqueda,
            Guid? zonaId,
            bool aplicarFiltroZona,
            int page,
            int pageSize);
    }
}
