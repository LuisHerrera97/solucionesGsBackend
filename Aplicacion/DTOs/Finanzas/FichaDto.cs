using System;

namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class FichaDto
    {
        public int Num { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Hora { get; set; }
        public string Folio { get; set; }
        public decimal Capital { get; set; }
        public decimal Interes { get; set; }
        public decimal Total { get; set; }
        public decimal AbonoAcumulado { get; set; }
        public decimal MoraAcumulada { get; set; }
        public decimal SaldoCap { get; set; }
        public decimal SaldoPendiente { get; set; }
        public bool Pagada { get; set; }
        public bool Aplicado { get; set; }
    }
}
