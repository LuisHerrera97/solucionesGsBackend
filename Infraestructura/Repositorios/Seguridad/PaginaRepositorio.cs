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
    public class PaginaRepositorio : IPaginaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PaginaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Pagina> AddAsync(Pagina pagina)
        {
            await _context.Paginas.AddAsync(pagina);
            return pagina;
        }

        public async Task DeleteAsync(Guid id)
        {
            var pagina = await _context.Paginas.FindAsync(id);
            if (pagina != null)
            {
                _context.Paginas.Remove(pagina);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Paginas.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pagina>> GetAllAsync(int? page = null, int? pageSize = null)
        {
            var query = _context.Paginas
                .AsNoTracking()
                .Include(p => p.Modulo)
                .OrderBy(p => p.Orden)
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

        public async Task<Pagina> GetByIdAsync(Guid id)
        {
            return await _context.Paginas
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pagina> GetByClaveAsync(string clave)
        {
            return await _context.Paginas
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Clave == clave);
        }

        public async Task<IEnumerable<Pagina>> GetByModuloIdAsync(Guid moduloId)
        {
            return await _context.Paginas
                .AsNoTracking()
                .Where(p => p.IdModulo == moduloId)
                .OrderBy(p => p.Orden)
                .ToListAsync();
        }

        public async Task UpdateAsync(Pagina pagina)
        {
            _context.Paginas.Update(pagina);
            await Task.CompletedTask;
        }
    }
}
