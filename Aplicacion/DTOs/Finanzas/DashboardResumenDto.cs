using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class DashboardResumenDto
    {
        public int TotalClientes { get; set; }
        public int CreditosActivos { get; set; }
        public decimal TotalVencido { get; set; }
    }
}
