using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Cortes;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Cortes;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.Finanzas.Cortes
{
    public class CorteCajaRepositorio : ICorteCajaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public CorteCajaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CorteCaja> AddAsync(CorteCaja corte)
        {
            _context.CortesCaja.Add(corte);
            await Task.CompletedTask;
            return corte;
        }

        public async Task<string> ObtenerSiguienteFolioAsync(DateTime fecha)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose) await connection.OpenAsync();

            try
            {
                await using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT nextval('corte_caja_folio_seq')";
                var raw = await cmd.ExecuteScalarAsync();
                var seq = Convert.ToInt64(raw);
                return $"CC-{fecha:yyyyMMdd}-{seq:000000}";
            }
            finally
            {
                if (shouldClose) await connection.CloseAsync();
            }
        }

        public async Task<IEnumerable<CorteCaja>> ObtenerEnRangoAsync(DateTime fechaInicio, DateTime fechaFin, int? page = null, int? pageSize = null)
        {
            var desde = fechaInicio.Date;
            var hasta = fechaFin.Date;
            if (hasta < desde) (desde, hasta) = (hasta, desde);
            var hastaExclusive = hasta.AddDays(1);

            var query = _context.CortesCaja
                .AsNoTracking()
                .Include(c => c.Movimientos)
                .Where(c => c.Fecha >= desde && c.Fecha < hastaExclusive)
                .OrderByDescending(c => c.Fecha)
                .ThenByDescending(c => c.Hora)
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
    }
}
