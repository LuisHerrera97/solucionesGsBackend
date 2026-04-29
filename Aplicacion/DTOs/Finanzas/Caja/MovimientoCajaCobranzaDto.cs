using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas.Caja
{
    /// <summary>
    /// Vista mínima de movimientos de caja para la pantalla Cobranza (menos payload que <see cref="MovimientoCajaDto"/>).
    /// </summary>
    public class MovimientoCajaCobranzaDto
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Concepto { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public decimal? Abono { get; set; }
        public decimal? Mora { get; set; }
        public Guid? OperacionId { get; set; }
        public Guid? ReversaDeId { get; set; }
        public bool Revertido { get; set; }
        public Guid? CreditoId { get; set; }
        public string? CreditoFolio { get; set; }
        public string? ClienteNombre { get; set; }
        public int? NumeroFicha { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; } = string.Empty;
    }
}
