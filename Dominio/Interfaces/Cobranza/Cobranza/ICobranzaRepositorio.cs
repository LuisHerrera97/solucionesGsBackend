using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Cobranza.Cobranza;

namespace FinancieraSoluciones.Domain.Interfaces.Cobranza.Cobranza
{
    public interface ICobranzaRepositorio
    {
        Task<IEnumerable<MovimientoCobranza>> ObtenerAsync(
            DateTime fechaInicio,
            DateTime fechaFin,
            string busqueda,
            int? page = null,
            int? pageSize = null,
            Guid? zonaId = null,
            bool aplicarFiltroZona = true);
    }
}
