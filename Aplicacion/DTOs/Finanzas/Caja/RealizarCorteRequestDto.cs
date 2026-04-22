using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas.Caja
{
    public class RealizarCorteRequestDto
    {
        public decimal TotalReal { get; set; }
        /// <summary>Día del corte (fecha local). Si no se envía, se usa el día calendario del servidor.</summary>
        public DateTime? FechaCorte { get; set; }
    }
}
