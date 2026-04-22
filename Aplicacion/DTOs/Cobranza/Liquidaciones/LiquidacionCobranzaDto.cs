using System;

namespace FinancieraSoluciones.Application.DTOs.Cobranza.Liquidaciones
{
    public class LiquidacionCobranzaDto
    {
        public Guid Id { get; set; }
        public Guid CobradorId { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public decimal TotalEfectivo { get; set; }
        public decimal TotalTransferencia { get; set; }
        public decimal Total { get; set; }
        public string Evidencia { get; set; }
        public string Estatus { get; set; }
        public string? NombreCobrador { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}

