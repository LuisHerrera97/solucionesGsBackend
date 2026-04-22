namespace FinancieraSoluciones.Application.DTOs.Cobranza.Liquidaciones
{
    public class LiquidacionPendienteResumenDto
    {
        public int CantidadMovimientos { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTarjeta { get; set; }
        public decimal TotalTransferencia { get; set; }
        public decimal Total { get; set; }
    }
}

