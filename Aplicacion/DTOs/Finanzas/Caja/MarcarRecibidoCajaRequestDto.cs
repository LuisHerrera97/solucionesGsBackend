using System;
using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.Finanzas.Caja
{
    public class MarcarRecibidoCajaRequestDto
    {
        public List<Guid> MovimientoIds { get; set; } = new List<Guid>();
        public DateTime? Fecha { get; set; }
    }
}
