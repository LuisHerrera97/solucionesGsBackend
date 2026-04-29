using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.Finanzas
{
    public class ClienteRepositorio : IClienteRepositorio
    {
        private readonly ApplicationDbContext _context;

        public ClienteRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Cliente> Items, int TotalCount)> GetAllAsync(int? page = null, int? pageSize = null, string? buscar = null, Guid? zonaId = null, bool aplicarFiltroZona = false)
        {
            var query = _context.Clientes
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Apellido)
                .AsQueryable();

            if (aplicarFiltroZona)
            {
                if (zonaId.HasValue)
                {
                    query = query.Where(c => c.IdZona == zonaId.Value);
                }
            }

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var term = $"%{buscar.Trim()}%";
                query = query.Where(c =>
                    EF.Functions.ILike(c.Nombre, term) ||
                    EF.Functions.ILike(c.Apellido, term) ||
                    EF.Functions.ILike(c.Nombre + " " + c.Apellido, term) ||
                    EF.Functions.ILike(c.Apellido + " " + c.Nombre, term) ||
                    (c.Negocio != null && EF.Functions.ILike(c.Negocio, term)) ||
                    (c.Zona != null && EF.Functions.ILike(c.Zona, term)) ||
                    (c.Direccion != null && EF.Functions.ILike(c.Direccion, term)));
            }

            var totalCount = await query.CountAsync();

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

            var items = await query.ToListAsync();
            return (items, totalCount);
        }

        public async Task<Cliente> GetByIdAsync(Guid id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task<Cliente> AddAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            return cliente;
        }

        public async Task UpdateAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }
        }
    }
}
