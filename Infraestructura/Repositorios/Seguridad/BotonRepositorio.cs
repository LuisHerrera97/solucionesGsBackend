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
    public class BotonRepositorio : IBotonRepositorio
    {
        private readonly ApplicationDbContext _context;

        public BotonRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Boton> AddAsync(Boton boton)
        {
            await _context.Botones.AddAsync(boton);
            return boton;
        }

        public async Task DeleteAsync(Guid id)
        {
            var boton = await _context.Botones.FindAsync(id);
            if (boton != null)
            {
                _context.Botones.Remove(boton);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Botones.AnyAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Boton>> GetAllAsync(int? page = null, int? pageSize = null)
        {
            var query = _context.Botones
                .AsNoTracking()
                .Include(b => b.Pagina)
                .ThenInclude(p => p.Modulo)
                .OrderBy(b => b.Orden)
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

        public async Task<Boton> GetByIdAsync(Guid id)
        {
            return await _context.Botones
                .AsNoTracking()
                .Include(b => b.Pagina)
                .ThenInclude(p => p.Modulo)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Boton> GetByClaveAsync(string clave)
        {
            return await _context.Botones
                .AsNoTracking()
                .Include(b => b.Pagina)
                .ThenInclude(p => p.Modulo)
                .FirstOrDefaultAsync(b => b.Clave == clave);
        }

        public async Task<IEnumerable<Boton>> GetByPaginaIdAsync(Guid paginaId)
        {
            return await _context.Botones
                .AsNoTracking()
                .Where(b => b.IdPagina == paginaId)
                .OrderBy(b => b.Orden)
                .ToListAsync();
        }

        public async Task UpdateAsync(Boton boton)
        {
            _context.Botones.Update(boton);
            await Task.CompletedTask;
        }
    }
}
