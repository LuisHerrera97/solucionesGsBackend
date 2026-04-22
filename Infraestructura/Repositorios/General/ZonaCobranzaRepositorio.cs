using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.General
{
    public class ZonaCobranzaRepositorio : IZonaCobranzaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public ZonaCobranzaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ZonaCobranza>> GetAllAsync(int? page = null, int? pageSize = null)
        {
            var query = _context.ZonasCobranza
                .AsNoTracking()
                .OrderBy(z => z.Orden)
                .ThenBy(z => z.Nombre)
                .AsQueryable();

            if (page.HasValue || pageSize.HasValue)
            {
                var normalizedPage = page.GetValueOrDefault(1);
                if (normalizedPage < 1) normalizedPage = 1;

                var normalizedPageSize = pageSize.GetValueOrDefault(100);
                if (normalizedPageSize < 1) normalizedPageSize = 1;
                if (normalizedPageSize > 500) normalizedPageSize = 500;

                var skip = (normalizedPage - 1) * normalizedPageSize;
                query = query.Skip(skip).Take(normalizedPageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<ZonaCobranza> GetByIdAsync(Guid id)
        {
            return await _context.ZonasCobranza.FindAsync(id);
        }

        public async Task<ZonaCobranza> AddAsync(ZonaCobranza zona)
        {
            await _context.ZonasCobranza.AddAsync(zona);
            return zona;
        }

        public async Task UpdateAsync(ZonaCobranza zona)
        {
            _context.ZonasCobranza.Update(zona);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var zona = await _context.ZonasCobranza.FindAsync(id);
            if (zona != null)
            {
                _context.ZonasCobranza.Remove(zona);
            }
        }

        public async Task<bool> ExistsByNombreAsync(string nombre)
        {
            var normalized = nombre.Trim().ToLower();
            return await _context.ZonasCobranza.AnyAsync(z => z.Nombre.ToLower() == normalized);
        }
    }
}
