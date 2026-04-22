using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IPasswordResetTokenRepositorio
    {
        Task<PasswordResetToken> AddAsync(PasswordResetToken token);
        Task<PasswordResetToken> GetValidAsync(Guid usuarioId, string codigo, DateTime nowUtc);
        Task InvalidateAllAsync(Guid usuarioId, DateTime nowUtc);
    }
}

