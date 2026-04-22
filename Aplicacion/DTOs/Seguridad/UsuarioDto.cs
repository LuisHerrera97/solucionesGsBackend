using System;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string UsuarioAcceso { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public DateTime? PasswordExpiresAt { get; set; }
        public bool MustChangePassword { get; set; }
        public DateTime? LockoutUntil { get; set; }
        public Guid IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public Guid? IdZonaCobranza { get; set; }
        public string NombreZonaCobranza { get; set; }
    }
}
