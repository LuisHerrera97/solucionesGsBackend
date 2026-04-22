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
    public class PermisoPaginaRepositorio : IPermisoPaginaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PermisoPaginaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PermisoPagina> AddAsync(PermisoPagina permisoPagina)
        {
            await _context.PermisosPagina.AddAsync(permisoPagina);
            return permisoPagina;
        }

        public async Task DeleteAsync(Guid id)
        {
            var permisoPagina = await _context.PermisosPagina.FindAsync(id);
            if (permisoPagina != null)
            {
                _context.PermisosPagina.Remove(permisoPagina);
            }
        }

        public async Task DeleteByPerfilIdAsync(Guid perfilId)
        {
            var permisos = await _context.PermisosPagina
                .Where(pp => pp.IdPerfil == perfilId)
                .ToListAsync();
            
            if (permisos.Any())
            {
                _context.PermisosPagina.RemoveRange(permisos);
            }
        }

        public async Task DeleteByPaginaIdAsync(Guid paginaId)
        {
            var permisos = await _context.PermisosPagina
                .Where(pp => pp.IdPagina == paginaId)
                .ToListAsync();

            if (permisos.Any())
            {
                _context.PermisosPagina.RemoveRange(permisos);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.PermisosPagina.AnyAsync(pp => pp.Id == id);
        }

        public async Task<IEnumerable<PermisoPagina>> GetAllAsync()
        {
            return await _context.PermisosPagina
                .AsNoTracking()
                .Include(pp => pp.Perfil)
                .Include(pp => pp.Pagina)
                .ThenInclude(p => p.Modulo)
                .ToListAsync();
        }

        public async Task<PermisoPagina> GetByIdAsync(Guid id)
        {
            return await _context.PermisosPagina
                .AsNoTracking()
                .Include(pp => pp.Perfil)
                .Include(pp => pp.Pagina)
                .ThenInclude(p => p.Modulo)
                .FirstOrDefaultAsync(pp => pp.Id == id);
        }

        public async Task<IEnumerable<PermisoPagina>> GetByPerfilIdAsync(Guid perfilId)
        {
            return await _context.PermisosPagina
                .AsNoTracking()
                .Where(pp => pp.IdPerfil == perfilId)
                .Include(pp => pp.Pagina)
                .ThenInclude(p => p.Modulo)
                .ToListAsync();
        }

        public async Task<PermisoPagina> GetByPerfilAndPaginaAsync(Guid perfilId, Guid paginaId)
        {
            return await _context.PermisosPagina
                .AsNoTracking()
                .FirstOrDefaultAsync(pp => pp.IdPerfil == perfilId && pp.IdPagina == paginaId);
        }

        public async Task UpdateAsync(PermisoPagina permisoPagina)
        {
            _context.PermisosPagina.Update(permisoPagina);
            await Task.CompletedTask;
        }
    }
}
