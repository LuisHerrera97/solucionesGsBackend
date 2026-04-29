using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas.Caja
{
    public class MovimientoCajaDto
    {
        public Guid Id { get; set; }
        public string Tipo { get; set; }
        public string Concepto { get; set; }
        public string Medio { get; set; }
        public decimal Total { get; set; }
        public decimal? MontoEfectivo { get; set; }
        public decimal? MontoTransferencia { get; set; }
        public decimal? Abono { get; set; }
        public decimal? Mora { get; set; }
        public Guid? CobradorId { get; set; }
        public Guid? CorteCajaId { get; set; }
        public bool RecibidoCaja { get; set; }
        public string? EstatusFichaFinanzas { get; set; }
        public Guid? ReversaDeId { get; set; }
        public Guid? OperacionId { get; set; }
        public bool Revertido { get; set; }
        public Guid? CreditoId { get; set; }
        public string? CreditoFolio { get; set; }
        public string? ClienteNombre { get; set; }
        public int? NumeroFicha { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public bool RegistraCaja { get; set; } = true;
    }
}

