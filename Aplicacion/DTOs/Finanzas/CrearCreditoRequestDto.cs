using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class CrearCreditoRequestDto
    {
        public Guid ClienteId { get; set; }
        public decimal Monto { get; set; }
        public int Plazo { get; set; }
        public string Tipo { get; set; }
        public bool? PermitirDomingo { get; set; }
        public bool? AplicarFeriados { get; set; }
        public decimal? TasaManual { get; set; }
        public string? Observacion { get; set; }
    }
}
