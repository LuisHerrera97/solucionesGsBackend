namespace FinancieraSoluciones.Application.DTOs.Finanzas.Caja
{
    public class ResumenEstadoFichaCajaDto
    {
        public decimal Total { get; set; }
        public decimal Pendiente { get; set; }
        public decimal Liquidado { get; set; }
        public decimal EnCorte { get; set; }
    }
}
