namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class AutenticacionResponseDto
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UsuarioDto Usuario { get; set; }
        public bool Autenticado { get; set; }
    }
}