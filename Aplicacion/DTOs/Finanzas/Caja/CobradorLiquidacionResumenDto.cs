using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas.Caja
{
    public class CobradorLiquidacionResumenDto
    {
        public Guid CobradorId { get; set; }
        public string NombreCobrador { get; set; } = string.Empty;
        public int CantidadMovimientos { get; set; }
        public decimal Total { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTarjeta { get; set; }
        public decimal TotalTransferencia { get; set; }
    }
}
