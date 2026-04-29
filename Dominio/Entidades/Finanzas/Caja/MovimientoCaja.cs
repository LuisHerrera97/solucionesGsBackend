using System;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Cortes;
using FinancieraSoluciones.Domain.Entidades.Finanzas;

namespace FinancieraSoluciones.Domain.Entidades.Finanzas.Caja
{
    public class MovimientoCaja
    {
        public Guid Id { get; set; }
        public Guid? CorteCajaId { get; set; }
        public CorteCaja CorteCaja { get; set; }
        public Guid? CobradorId { get; set; }
        /// <summary>
        /// Caja confirmó recepción del efectivo/medios (pendiente → cobrado). Excluye liquidación enviada sin confirmar.
        /// </summary>
        public bool RecibidoCaja { get; set; }
        public Guid? ReversaDeId { get; set; }
        public string? IdempotencyKey { get; set; }
        public string Tipo { get; set; }
        public string Concepto { get; set; }
        public string Medio { get; set; }
        public decimal Total { get; set; }
        public decimal? MontoEfectivo { get; set; }
        public decimal? MontoTransferencia { get; set; }
        public decimal? Abono { get; set; }
        public decimal? Mora { get; set; }
        public Guid? CreditoId { get; set; }
        public Credito? Credito { get; set; }
        public int? NumeroFicha { get; set; }
        public DateTime Fecha { get; set; }
        public string? Hora { get; set; }
        /// <summary>
        /// Si es false, el movimiento solo alimenta historial del crédito (p. ej. condonación de interés) y no entra en turno/corte/rango de caja.
        /// </summary>
        public bool RegistraCaja { get; set; } = true;
    }
}
