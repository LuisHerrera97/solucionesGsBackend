using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.Finanzas.Caja
{
    public class ResumenLiquidacionesCajaDto
    {
        public List<CobradorLiquidacionResumenDto> Cobradores { get; set; } = new List<CobradorLiquidacionResumenDto>();
        public ResumenEstadoFichaCajaDto Estados { get; set; } = new ResumenEstadoFichaCajaDto();
    }
}
