using System.ComponentModel.DataAnnotations;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class UsuarioCrearDto : UsuarioDto
    {
        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Contrasena { get; set; }
    }
}
