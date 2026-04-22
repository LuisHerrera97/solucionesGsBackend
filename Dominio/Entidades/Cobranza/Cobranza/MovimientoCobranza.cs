using System;

namespace FinancieraSoluciones.Domain.Entidades.Cobranza.Cobranza
{
    public class MovimientoCobranza
    {
        public Guid CreditoId { get; set; }
        public string CreditoFolio { get; set; }
        public int NumFicha { get; set; }
        public DateTime FechaPago { get; set; }
        public string HoraPago { get; set; }
        public DateTime FechaLimite { get; set; }
        public decimal Capital { get; set; }
        public decimal Interes { get; set; }
        public decimal Abono { get; set; }
        public decimal Mora { get; set; }
        public decimal TotalCobrado { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteNegocio { get; set; }
        public string TipoCredito { get; set; }
    }
}
