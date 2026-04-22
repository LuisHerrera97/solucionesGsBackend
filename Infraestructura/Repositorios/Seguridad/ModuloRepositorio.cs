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
    public class ModuloRepositorio : IModuloRepositorio
    {
        private readonly ApplicationDbContext _context;

        public ModuloRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Modulo> AddAsync(Modulo modulo)
        {
            await _context.Modulos.AddAsync(modulo);
            return modulo;
        }

        public async Task DeleteAsync(Guid id)
        {
            var modulo = await _context.Modulos.FindAsync(id);
            if (modulo != null)
            {
                _context.Modulos.Remove(modulo);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Modulos.AnyAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Modulo>> GetAllAsync(int? page = null, int? pageSize = null)
        {
            var query = _context.Modulos
                .AsNoTracking()
                .OrderBy(m => m.Orden)
                .ThenBy(m => m.Nombre)
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

        public async Task<Modulo> GetByIdAsync(Guid id)
        {
            return await _context.Modulos.FindAsync(id);
        }

        public async Task<Modulo> GetByClaveAsync(string clave)
        {
            return await _context.Modulos.FirstOrDefaultAsync(m => m.Clave == clave);
        }

        public async Task UpdateAsync(Modulo modulo)
        {
            _context.Modulos.Update(modulo);
            await Task.CompletedTask;
        }
    }
}
