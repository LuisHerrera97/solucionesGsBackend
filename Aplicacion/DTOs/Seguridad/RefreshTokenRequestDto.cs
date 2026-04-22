namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class RefreshTokenRequestDto
    {
        public Guid UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}
