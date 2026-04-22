using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class CreditoResumenDto
    {
        public Guid Id { get; set; }
        public string Folio { get; set; }
        public decimal Monto { get; set; }
        public decimal Total { get; set; }
        public decimal Pagado { get; set; }
        public string Tipo { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
