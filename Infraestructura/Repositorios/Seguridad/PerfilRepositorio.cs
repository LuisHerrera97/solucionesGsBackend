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
    public class PerfilRepositorio : IPerfilRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PerfilRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Perfil> AddAsync(Perfil perfil)
        {
            await _context.Perfiles.AddAsync(perfil);
            return perfil;
        }

        public async Task DeleteAsync(Guid id)
        {
            var perfil = await _context.Perfiles.FindAsync(id);
            if (perfil != null)
            {
                _context.Perfiles.Remove(perfil);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Perfiles.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Perfil>> GetAllAsync(int? page = null, int? pageSize = null)
        {
            var query = _context.Perfiles
                .AsNoTracking()
                .OrderBy(p => p.Orden)
                .ThenBy(p => p.Nombre)
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

        public async Task<Perfil> GetByIdAsync(Guid id)
        {
            return await _context.Perfiles.FindAsync(id);
        }

        public async Task<Perfil> GetByClaveAsync(string clave)
        {
            return await _context.Perfiles.FirstOrDefaultAsync(p => p.Clave == clave);
        }

        public async Task UpdateAsync(Perfil perfil)
        {
            _context.Perfiles.Update(perfil);
            await Task.CompletedTask;
        }
    }
}
