using System;
using System.Collections.Generic;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;

namespace FinancieraSoluciones.Application.DTOs.Finanzas.Cortes
{
    public class CorteCajaDto
    {
        public Guid Id { get; set; }
        public string Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public decimal TotalTeorico { get; set; }
        public decimal TotalReal { get; set; }
        public decimal Diferencia { get; set; }
        public Guid RealizadoPorId { get; set; }
        public List<MovimientoCajaDto> Movimientos { get; set; } = new List<MovimientoCajaDto>();
    }
}
