using System;
using System.Collections.Generic;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;

namespace FinancieraSoluciones.Domain.Entidades.Finanzas.Cortes
{
    public class CorteCaja
    {
        public Guid Id { get; set; }
        public string Folio { get; set; }
        public DateTime Fecha { get; set; }
        public string? Hora { get; set; }
        public decimal TotalTeorico { get; set; }
        public decimal TotalReal { get; set; }
        public decimal Diferencia { get; set; }
        public Guid RealizadoPorId { get; set; }
        public List<MovimientoCaja> Movimientos { get; set; } = new List<MovimientoCaja>();
    }
}
