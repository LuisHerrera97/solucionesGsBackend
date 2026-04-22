namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class RestablecerPasswordRequestDto
    {
        public string UsuarioAcceso { get; set; }
        public string Codigo { get; set; }
        public string NuevaContrasena { get; set; }
    }
}

