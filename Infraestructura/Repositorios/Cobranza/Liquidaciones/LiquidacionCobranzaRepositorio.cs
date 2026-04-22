using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.Cobranza.Liquidaciones
{
    public class LiquidacionCobranzaRepositorio : ILiquidacionCobranzaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public LiquidacionCobranzaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LiquidacionCobranza> AddAsync(LiquidacionCobranza liquidacion)
        {
            await _context.LiquidacionesCobranza.AddAsync(liquidacion);
            return liquidacion;
        }

        public async Task<LiquidacionCobranza> GetByIdAsync(Guid id)
        {
            return await _context.LiquidacionesCobranza
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LiquidacionCobranza>> GetByCobradorAsync(Guid cobradorId, DateTime fechaInicio, DateTime fechaFin)
        {
            var desde = fechaInicio.Date;
            var hasta = fechaFin.Date;
            if (hasta < desde) (desde, hasta) = (hasta, desde);
            var hastaExclusive = hasta.AddDays(1);

            return await _context.LiquidacionesCobranza
                .AsNoTracking()
                .Where(l => l.CobradorId == cobradorId && l.Fecha >= desde && l.Fecha < hastaExclusive)
                .OrderByDescending(l => l.Fecha)
                .ThenByDescending(l => l.Hora)
                .ToListAsync();
        }

        public async Task<IEnumerable<LiquidacionCobranza>> GetTodasAsync(DateTime fechaInicio, DateTime fechaFin, Guid? zonaId = null)
        {
            var desde = fechaInicio.Date;
            var hasta = fechaFin.Date;
            if (hasta < desde) (desde, hasta) = (hasta, desde);
            var hastaExclusive = hasta.AddDays(1);

            var query = _context.LiquidacionesCobranza
                .AsNoTracking()
                .Where(l => l.Fecha >= desde && l.Fecha < hastaExclusive)
                .AsQueryable();

            if (zonaId.HasValue)
            {
                var cobradoresZona = _context.Usuarios
                    .AsNoTracking()
                    .Where(u => u.IdZonaCobranza == zonaId.Value)
                    .Select(u => u.Id);
                query = query.Where(l => cobradoresZona.Contains(l.CobradorId));
            }

            return await query
                .OrderByDescending(l => l.Fecha)
                .ThenByDescending(l => l.Hora)
                .ToListAsync();
        }

        public async Task UpdateAsync(LiquidacionCobranza liquidacion)
        {
            _context.LiquidacionesCobranza.Update(liquidacion);
            await Task.CompletedTask;
        }

        public async Task<Dictionary<Guid, string>> GetEstatusPorIdsAsync(IEnumerable<Guid> ids)
        {
            var set = ids.Distinct().ToArray();
            if (set.Length == 0)
            {
                return new Dictionary<Guid, string>();
            }

            return await _context.LiquidacionesCobranza
                .AsNoTracking()
                .Where(l => set.Contains(l.Id))
                .ToDictionaryAsync(l => l.Id, l => l.Estatus);
        }
    }
}

