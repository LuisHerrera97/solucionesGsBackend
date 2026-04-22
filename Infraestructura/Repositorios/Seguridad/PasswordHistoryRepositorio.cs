using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.Seguridad
{
    public class PasswordHistoryRepositorio : IPasswordHistoryRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PasswordHistoryRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PasswordHistory> AddAsync(PasswordHistory history)
        {
            await _context.PasswordHistories.AddAsync(history);
            return history;
        }

        public async Task<IEnumerable<PasswordHistory>> GetRecentAsync(Guid usuarioId, int take)
        {
            var t = take <= 0 ? 0 : take;
            return await _context.PasswordHistories
                .AsNoTracking()
                .Where(h => h.UsuarioId == usuarioId)
                .OrderByDescending(h => h.CreatedAt)
                .Take(t)
                .ToListAsync();
        }
    }
}

