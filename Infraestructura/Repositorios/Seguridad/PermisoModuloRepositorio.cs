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
    public class PermisoModuloRepositorio : IPermisoModuloRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PermisoModuloRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PermisoModulo> AddAsync(PermisoModulo permisoModulo)
        {
            await _context.PermisosModulo.AddAsync(permisoModulo);
            return permisoModulo;
        }

        public async Task DeleteAsync(Guid id)
        {
            var permisoModulo = await _context.PermisosModulo.FindAsync(id);
            if (permisoModulo != null)
            {
                _context.PermisosModulo.Remove(permisoModulo);
            }
        }

        public async Task DeleteByPerfilIdAsync(Guid perfilId)
        {
            var permisos = await _context.PermisosModulo
                .Where(pm => pm.IdPerfil == perfilId)
                .ToListAsync();
            
            if (permisos.Any())
            {
                _context.PermisosModulo.RemoveRange(permisos);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.PermisosModulo.AnyAsync(pm => pm.Id == id);
        }

        public async Task<IEnumerable<PermisoModulo>> GetAllAsync()
        {
            return await _context.PermisosModulo
                .AsNoTracking()
                .Include(pm => pm.Perfil)
                .Include(pm => pm.Modulo)
                .ToListAsync();
        }

        public async Task<PermisoModulo> GetByIdAsync(Guid id)
        {
            return await _context.PermisosModulo
                .AsNoTracking()
                .Include(pm => pm.Perfil)
                .Include(pm => pm.Modulo)
                .FirstOrDefaultAsync(pm => pm.Id == id);
        }

        public async Task<IEnumerable<PermisoModulo>> GetByPerfilIdAsync(Guid perfilId)
        {
            return await _context.PermisosModulo
                .AsNoTracking()
                .Where(pm => pm.IdPerfil == perfilId)
                .Include(pm => pm.Modulo)
                .ToListAsync();
        }

        public async Task<PermisoModulo> GetByPerfilAndModuloAsync(Guid perfilId, Guid moduloId)
        {
            return await _context.PermisosModulo
                .AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.IdPerfil == perfilId && pm.IdModulo == moduloId);
        }

        public async Task UpdateAsync(PermisoModulo permisoModulo)
        {
            _context.PermisosModulo.Update(permisoModulo);
            await Task.CompletedTask;
        }
    }
}
