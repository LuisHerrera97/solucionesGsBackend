using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class ReestructurarCreditoRequestDto
    {
        public Guid CreditoId { get; set; }
        public decimal NuevoMonto { get; set; }
        public int NuevoPlazo { get; set; }
        public string Tipo { get; set; }
    }
}

