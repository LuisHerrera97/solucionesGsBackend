using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class ClienteCreditosDto
    {
        public ClienteDto Cliente { get; set; }
        public List<CreditoResumenDto> Vigentes { get; set; } = new List<CreditoResumenDto>();
        public List<CreditoResumenDto> Liquidados { get; set; } = new List<CreditoResumenDto>();
    }
}

