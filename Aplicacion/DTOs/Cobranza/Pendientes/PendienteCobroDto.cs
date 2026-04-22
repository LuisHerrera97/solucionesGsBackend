using System;

namespace FinancieraSoluciones.Application.DTOs.Cobranza.Pendientes
{
    public class PendienteCobroDto
    {
        public Guid CreditoId { get; set; }
        public string CreditoFolio { get; set; }
        public int NumFicha { get; set; }
        public DateTime FechaLimite { get; set; }
        public decimal TotalFicha { get; set; }
        public decimal AbonoAcumulado { get; set; }
        public decimal Pendiente { get; set; }
        public string Estado { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteNegocio { get; set; }
        public string TipoCredito { get; set; }
    }
}
