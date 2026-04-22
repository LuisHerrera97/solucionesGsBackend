using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas.Caja
{
    public class LiquidarCobradorRequestDto
    {
        public Guid CobradorId { get; set; }
        public string? Evidencia { get; set; }
    }
}
