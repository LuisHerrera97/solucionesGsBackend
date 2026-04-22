using System.ComponentModel.DataAnnotations;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class ResetPasswordAdminRequestDto
    {
        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        public string NuevaContrasena { get; set; }
    }
}
