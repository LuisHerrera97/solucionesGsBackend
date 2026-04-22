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
    public class PermisoBotonRepositorio : IPermisoBotonRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PermisoBotonRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PermisoBoton> AddAsync(PermisoBoton permisoBoton)
        {
            await _context.PermisosBoton.AddAsync(permisoBoton);
            return permisoBoton;
        }

        public async Task DeleteAsync(Guid id)
        {
            var permisoBoton = await _context.PermisosBoton.FindAsync(id);
            if (permisoBoton != null)
            {
                _context.PermisosBoton.Remove(permisoBoton);
            }
        }

        public async Task DeleteByPerfilIdAsync(Guid perfilId)
        {
            var permisos = await _context.PermisosBoton
                .Where(pb => pb.IdPerfil == perfilId)
                .ToListAsync();
            
            if (permisos.Any())
            {
                _context.PermisosBoton.RemoveRange(permisos);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.PermisosBoton.AnyAsync(pb => pb.Id == id);
        }

        public async Task<IEnumerable<PermisoBoton>> GetAllAsync()
        {
            return await _context.PermisosBoton
                .AsNoTracking()
                .Include(pb => pb.Perfil)
                .Include(pb => pb.Boton)
                .ThenInclude(b => b.Pagina)
                .ThenInclude(p => p.Modulo)
                .ToListAsync();
        }

        public async Task<PermisoBoton> GetByIdAsync(Guid id)
        {
            return await _context.PermisosBoton
                .AsNoTracking()
                .Include(pb => pb.Perfil)
                .Include(pb => pb.Boton)
                .ThenInclude(b => b.Pagina)
                .ThenInclude(p => p.Modulo)
                .FirstOrDefaultAsync(pb => pb.Id == id);
        }

        public async Task<IEnumerable<PermisoBoton>> GetByPerfilIdAsync(Guid perfilId)
        {
            return await _context.PermisosBoton
                .AsNoTracking()
                .Where(pb => pb.IdPerfil == perfilId)
                .Include(pb => pb.Boton)
                .ThenInclude(b => b.Pagina)
                .ThenInclude(p => p.Modulo)
                .ToListAsync();
        }

        public async Task<PermisoBoton> GetByPerfilAndBotonAsync(Guid perfilId, Guid botonId)
        {
            return await _context.PermisosBoton
                .AsNoTracking()
                .FirstOrDefaultAsync(pb => pb.IdPerfil == perfilId && pb.IdBoton == botonId);
        }

        public async Task<bool> HasPermisoAsync(Guid perfilId, string botonClave)
        {
            if (string.IsNullOrWhiteSpace(botonClave))
            {
                return false;
            }

            return await _context.PermisosBoton
                .AsNoTracking()
                .Where(pb => pb.IdPerfil == perfilId && pb.TienePermiso)
                .Join(
                    _context.Botones.AsNoTracking(),
                    pb => pb.IdBoton,
                    b => b.Id,
                    (pb, b) => new { pb, b })
                .AnyAsync(x => x.b.Clave == botonClave && x.b.Activo);
        }

        public async Task<bool> HasPermisoAlgunoAsync(Guid perfilId, params string[] botonClaves)
        {
            if (botonClaves == null || botonClaves.Length == 0)
            {
                return false;
            }

            var claves = botonClaves.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.Trim()).Distinct().ToList();
            if (claves.Count == 0)
            {
                return false;
            }

            return await _context.PermisosBoton
                .AsNoTracking()
                .Where(pb => pb.IdPerfil == perfilId && pb.TienePermiso)
                .Join(
                    _context.Botones.AsNoTracking(),
                    pb => pb.IdBoton,
                    b => b.Id,
                    (pb, b) => b)
                .AnyAsync(b => b.Activo && claves.Contains(b.Clave));
        }

        public async Task UpdateAsync(PermisoBoton permisoBoton)
        {
            _context.PermisosBoton.Update(permisoBoton);
            await Task.CompletedTask;
        }
    }
}
