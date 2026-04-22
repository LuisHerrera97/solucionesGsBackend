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
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Usuario>> GetByIdsAsync(IReadOnlyCollection<Guid> ids)
        {
            if (ids == null || ids.Count == 0) return Array.Empty<Usuario>();
            var idSet = ids.ToHashSet();
            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => idSet.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<Usuario> AddAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            return usuario;
        }

        public async Task DeleteAsync(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsByUsuarioAccesoAsync(string usuarioAcceso)
        {
            return await _context.Usuarios.AnyAsync(u => u.UsuarioAcceso == usuarioAcceso);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync(int? page = null, int? pageSize = null)
        {
            var query = _context.Usuarios
                .AsNoTracking()
                .OrderBy(u => u.Nombre)
                .ThenBy(u => u.ApellidoPaterno)
                .ThenBy(u => u.UsuarioAcceso)
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

        public async Task<Usuario> GetByIdAsync(Guid id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task<Usuario> GetByUsuarioAccesoAsync(string usuarioAcceso)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioAcceso == usuarioAcceso);
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await Task.CompletedTask;
        }
    }
}
