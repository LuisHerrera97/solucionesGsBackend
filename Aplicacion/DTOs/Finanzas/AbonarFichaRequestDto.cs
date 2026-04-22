namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class AbonarFichaRequestDto
    {
        public string? IdempotencyKey { get; set; }
        public decimal? MontoAbono { get; set; }
        public string Medio { get; set; }
        public decimal? MontoEfectivo { get; set; }
        public decimal? MontoTransferencia { get; set; }
    }
}
