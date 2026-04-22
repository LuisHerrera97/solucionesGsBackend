using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class CondonarInteresRequestDto
    {
        public Guid CreditoId { get; set; }
        public int NumeroFicha { get; set; }
    }
}
