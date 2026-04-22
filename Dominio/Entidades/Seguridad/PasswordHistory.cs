using System;

namespace FinancieraSoluciones.Domain.Entidades.Seguridad
{
    public class PasswordHistory
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

