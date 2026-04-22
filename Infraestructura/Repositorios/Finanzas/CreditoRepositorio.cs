using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.Finanzas
{
    public class CreditoRepositorio : ICreditoRepositorio
    {
        private readonly ApplicationDbContext _context;

        public CreditoRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Credito>> GetAllAsync(string? searchTerm = null, int? page = null, int? pageSize = null, Guid? zonaId = null, bool aplicarFiltroZona = false)
        {
            var query = _context.Creditos
                .Include(c => c.Cliente)
                .AsNoTracking()
                .OrderByDescending(c => c.FechaCreacion)
                .AsQueryable();

            if (aplicarFiltroZona)
            {
                if (zonaId.HasValue)
                {
                    query = query.Where(c => c.Cliente.IdZona == zonaId.Value);
                }
                else
                {
                    query = query.Where(c => false);
                }
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = $"%{searchTerm}%";
                query = query.Where(c =>
                    EF.Functions.ILike(c.Folio, term) ||
                    EF.Functions.ILike(c.Cliente.Nombre, term) ||
                    EF.Functions.ILike(c.Cliente.Apellido, term) ||
                    EF.Functions.ILike(c.Cliente.Nombre + " " + c.Cliente.Apellido, term) ||
                    EF.Functions.ILike(c.Cliente.Apellido + " " + c.Cliente.Nombre, term));
            }

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

        public async Task<Credito> GetByIdAsync(Guid id)
        {
            return await _context.Creditos
                .Include(c => c.Cliente)
                .Include(c => c.Fichas)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Credito>> GetByClienteIdAsync(Guid clienteId)
        {
            return await _context.Creditos
                .AsNoTracking()
                .Where(c => c.ClienteId == clienteId)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<int> ContarPorClienteIdAsync(Guid clienteId)
        {
            return await _context.Creditos.AsNoTracking().CountAsync(c => c.ClienteId == clienteId);
        }

        public async Task<string> ObtenerSiguienteFolioAsync(DateTime fecha)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose) await connection.OpenAsync();

            try
            {
                await using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT nextval('credito_folio_seq')";
                var raw = await cmd.ExecuteScalarAsync();
                var seq = Convert.ToInt64(raw);
                return $"CR-{fecha:yyyyMMdd}-{seq:000000}";
            }
            finally
            {
                if (shouldClose) await connection.CloseAsync();
            }
        }

        public async Task<IEnumerable<Ficha>> GetFichasVencidasAsync(DateTime hoy)
        {
            var fecha = hoy.Date;
            return await _context.Fichas
                .Include(f => f.Credito)
                    .ThenInclude(c => c.Cliente)
                .AsNoTracking()
                .Where(f => !f.Pagada && f.Fecha < fecha)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ficha>> GetFichasPendientesParaMoraAsync(DateTime hoy)
        {
            var fecha = hoy.Date;
            return await _context.Fichas
                .Include(f => f.Credito)
                .Where(f => !f.Pagada && f.Fecha < fecha)
                .ToListAsync();
        }

        public async Task<Credito> AddAsync(Credito credito)
        {
            await _context.Creditos.AddAsync(credito);
            return credito;
        }

        public async Task UpdateAsync(Credito credito)
        {
            _context.Creditos.Update(credito);
            await Task.CompletedTask;
        }
    }
}
