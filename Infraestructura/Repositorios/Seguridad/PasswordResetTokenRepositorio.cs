using System;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.Seguridad
{
    public class PasswordResetTokenRepositorio : IPasswordResetTokenRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PasswordResetTokenRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PasswordResetToken> AddAsync(PasswordResetToken token)
        {
            await _context.PasswordResetTokens.AddAsync(token);
            return token;
        }

        public async Task<PasswordResetToken> GetValidAsync(Guid usuarioId, string codigo, DateTime nowUtc)
        {
            var c = (codigo ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(c)) return null;

            return await _context.PasswordResetTokens
                .FirstOrDefaultAsync(t =>
                    t.UsuarioId == usuarioId &&
                    t.Codigo == c &&
                    t.UsedAt == null &&
                    t.ExpiresAt > nowUtc);
        }

        public async Task InvalidateAllAsync(Guid usuarioId, DateTime nowUtc)
        {
            var tokens = await _context.PasswordResetTokens
                .Where(t => t.UsuarioId == usuarioId && t.UsedAt == null && t.ExpiresAt > nowUtc)
                .ToListAsync();

            foreach (var t in tokens)
            {
                t.UsedAt = nowUtc;
            }
        }
    }
}

