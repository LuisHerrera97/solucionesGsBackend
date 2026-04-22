using System;
using FinancieraSoluciones.Domain.Entidades.General;

namespace FinancieraSoluciones.Domain.Entidades.Seguridad
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string UsuarioAcceso { get; set; }
        public string Contrasena { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public DateTime? PasswordChangedAt { get; set; }
        public DateTime? PasswordExpiresAt { get; set; }
        public bool MustChangePassword { get; set; }
        public int FailedLoginCount { get; set; }
        public DateTime? LockoutUntil { get; set; }
        public Guid IdPerfil { get; set; }
        public Guid? IdZonaCobranza { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Perfil Perfil { get; set; }
        public ZonaCobranza? ZonaCobranza { get; set; }
    }
}
