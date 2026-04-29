using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.Finanzas.Caja
{
    public class MovimientoCajaRepositorio : IMovimientoCajaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public MovimientoCajaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovimientoCaja>> ObtenerTurnoAsync(DateTime? fechaDia = null)
        {
            var desde = (fechaDia ?? DateTime.Today).Date;
            var hasta = desde.AddDays(1);
            return await _context.MovimientosCaja
                .AsNoTracking()
                .Where(m =>
                    m.RegistraCaja &&
                    m.CorteCajaId == null &&
                    m.Fecha >= desde &&
                    m.Fecha < hasta &&
                    (m.CobradorId == null || m.RecibidoCaja))
                .OrderBy(m => m.Fecha)
                .ThenBy(m => m.Hora)
                .ToListAsync();
        }

        public async Task<IEnumerable<MovimientoCaja>> ObtenerEnRangoAsync(
            DateTime fechaDesde,
            DateTime fechaHasta,
            int? page = null,
            int? pageSize = null,
            Guid? cobradorId = null,
            Guid? zonaId = null)
        {
            var desde = fechaDesde.Date;
            var hasta = fechaHasta.Date;
            if (hasta < desde) (desde, hasta) = (hasta, desde);
            var hastaExclusive = hasta.AddDays(1);

            var query = _context.MovimientosCaja
                .AsNoTracking()
                .Where(m => m.RegistraCaja && m.Fecha >= desde && m.Fecha < hastaExclusive)
                .OrderBy(m => m.Fecha)
                .ThenBy(m => m.Hora)
                .AsQueryable();

            if (cobradorId.HasValue)
            {
                query = query.Where(m => m.CobradorId == cobradorId.Value);
            }

            if (zonaId.HasValue)
            {
                var cobradoresZona = _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.IdZonaCobranza == zonaId.Value)
                    .Select(u => u.Id);
                query = query.Where(m => m.CobradorId.HasValue && cobradoresZona.Contains(m.CobradorId.Value));
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

        public async Task<IEnumerable<MovimientoCaja>> ObtenerPendientesLiquidacionAsync(Guid cobradorId, DateTime fecha)
        {
            var desde = fecha.Date;
            var hasta = desde.AddDays(1);
            var tipoFicha = TipoMovimientoCaja.Ficha.ToStoredString();
            var tipoIngreso = TipoMovimientoCaja.Ingreso.ToStoredString();
            return await _context.MovimientosCaja
                .AsNoTracking()
                .Where(m =>
                    m.RegistraCaja &&
                    m.CobradorId == cobradorId &&
                    m.CorteCajaId == null &&
                    m.Fecha >= desde &&
                    m.Fecha < hasta &&
                    (m.Tipo == tipoFicha || m.Tipo == tipoIngreso))
                .OrderBy(m => m.Fecha)
                .ThenBy(m => m.Hora)
                .ToListAsync();
        }

        public async Task<MovimientoCaja> AddAsync(MovimientoCaja movimiento)
        {
            _context.MovimientosCaja.Add(movimiento);
            await Task.CompletedTask;
            return movimiento;
        }

        public async Task<MovimientoCaja> GetByIdAsync(Guid id)
        {
            return await _context.MovimientosCaja.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<MovimientoCaja?> GetByIdempotencyKeyAsync(string idempotencyKey)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey)) return null;
            var key = idempotencyKey.Trim();
            return await _context.MovimientosCaja
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdempotencyKey == key);
        }

        public async Task<bool> TienePendientesCorteAsync(DateTime fecha)
        {
            var desde = fecha.Date;
            var hasta = desde.AddDays(1);
            return await _context.MovimientosCaja
                .AsNoTracking()
                .AnyAsync(m =>
                    m.RegistraCaja &&
                    m.CorteCajaId == null &&
                    m.Fecha >= desde &&
                    m.Fecha < hasta &&
                    (m.CobradorId == null || m.RecibidoCaja));
        }

        public async Task<bool> TienePendientesLiquidacionAsync(DateTime fecha)
        {
            var desde = fecha.Date;
            var hasta = desde.AddDays(1);
            var tipoFicha = TipoMovimientoCaja.Ficha.ToStoredString();
            var tipoIngreso = TipoMovimientoCaja.Ingreso.ToStoredString();
            return await _context.MovimientosCaja
                .AsNoTracking()
                .AnyAsync(m =>
                    m.RegistraCaja &&
                    m.CobradorId != null &&
                    m.CorteCajaId == null &&
                    m.Fecha >= desde &&
                    m.Fecha < hasta &&
                    (m.Tipo == tipoFicha || m.Tipo == tipoIngreso));
        }

        public async Task<int> AsignarCorteAsync(Guid corteId, DateTime fechaCorte)
        {
            var desde = fechaCorte.Date;
            var hasta = desde.AddDays(1);
            var movimientos = await _context.MovimientosCaja
                .Where(m =>
                    m.RegistraCaja &&
                    m.CorteCajaId == null &&
                    m.Fecha >= desde &&
                    m.Fecha < hasta &&
                    (m.CobradorId == null || m.RecibidoCaja))
                .ToListAsync();

            foreach (var m in movimientos)
            {
                m.CorteCajaId = corteId;
            }

            await Task.CompletedTask;
            return movimientos.Count;
        }

        public async Task<int> AsignarLiquidacionAsync(Guid liquidacionId, Guid cobradorId, DateTime fecha)
        {
            await Task.CompletedTask;
            return 0;
        }

        public async Task<int> DesvincularLiquidacionAsync(Guid liquidacionId)
        {
            await Task.CompletedTask;
            return 0;
        }

        public async Task<IEnumerable<MovimientoCaja>> ObtenerPorCreditoAsync(Guid creditoId)
        {
            return await _context.MovimientosCaja
                .AsNoTracking()
                .Where(m => m.CreditoId == creditoId)
                .OrderByDescending(m => m.Fecha)
                .ThenByDescending(m => m.Hora)
                .ToListAsync();
        }

        public async Task<int> MarcarRecibidoCajaAsync(IEnumerable<Guid> movimientoIds, DateTime fechaDia)
        {
            var idList = movimientoIds.Distinct().ToList();
            if (idList.Count == 0)
            {
                return 0;
            }

            var desde = fechaDia.Date;
            var hasta = desde.AddDays(1);
            var tipoFicha = TipoMovimientoCaja.Ficha.ToStoredString();
            var tipoIngreso = TipoMovimientoCaja.Ingreso.ToStoredString();
            var movimientos = await _context.MovimientosCaja
                .Where(m =>
                    idList.Contains(m.Id) &&
                    m.RegistraCaja &&
                    m.CobradorId != null &&
                    !m.RecibidoCaja &&
                    m.CorteCajaId == null &&
                    m.Fecha >= desde &&
                    m.Fecha < hasta &&
                    (m.Tipo == tipoFicha || m.Tipo == tipoIngreso))
                .ToListAsync();

            foreach (var m in movimientos)
            {
                m.RecibidoCaja = true;
            }

            await Task.CompletedTask;
            return movimientos.Count;
        }
    }
}
