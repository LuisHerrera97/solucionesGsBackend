namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class PenalizarFichaRequestDto
    {
        public decimal Monto { get; set; }
        public string? IdempotencyKey { get; set; }
    }
}
