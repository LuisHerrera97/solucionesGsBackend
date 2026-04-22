using System;

namespace FinancieraSoluciones.Domain.Entidades.Seguridad
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Codigo { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

