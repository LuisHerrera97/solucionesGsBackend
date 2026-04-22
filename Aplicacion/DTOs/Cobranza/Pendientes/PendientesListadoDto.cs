using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.Cobranza.Pendientes
{
    public class PendientesListadoDto
    {
        public List<PendienteCobroDto> Items { get; set; } = new List<PendienteCobroDto>();
        public int TotalCount { get; set; }
    }
}
