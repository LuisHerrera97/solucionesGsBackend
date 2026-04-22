using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.General
{
    public class AuditoriaEventoRepositorio : IAuditoriaEventoRepositorio
    {
        private readonly ApplicationDbContext _context;

        public AuditoriaEventoRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuditoriaEvento> AddAsync(AuditoriaEvento evento)
        {
            await _context.AuditoriaEventos.AddAsync(evento);
            return evento;
        }

        public async Task<IEnumerable<AuditoriaEvento>> GetAsync(DateTime desdeUtc, DateTime hastaUtc, Guid? usuarioId, string accion, string entidadTipo, Guid? entidadId, int? page, int? pageSize)
        {
            var desde = DateTime.SpecifyKind(desdeUtc, DateTimeKind.Utc);
            var hasta = DateTime.SpecifyKind(hastaUtc, DateTimeKind.Utc);
            if (hasta <= desde) hasta = desde.AddDays(1);

            var q = _context.AuditoriaEventos
                .AsNoTracking()
                .Where(e => e.Fecha >= desde && e.Fecha < hasta)
                .AsQueryable();

            if (usuarioId.HasValue) q = q.Where(e => e.UsuarioId == usuarioId.Value);
            if (!string.IsNullOrWhiteSpace(accion)) q = q.Where(e => e.Accion == accion.Trim());
            if (!string.IsNullOrWhiteSpace(entidadTipo)) q = q.Where(e => e.EntidadTipo == entidadTipo.Trim());
            if (entidadId.HasValue) q = q.Where(e => e.EntidadId == entidadId.Value);

            q = q.OrderByDescending(e => e.Fecha);

            if (page.HasValue || pageSize.HasValue)
            {
                var p = page.GetValueOrDefault(1);
                if (p < 1) p = 1;
                var ps = pageSize.GetValueOrDefault(100);
                if (ps < 1) ps = 1;
                if (ps > 500) ps = 500;
                q = q.Skip((p - 1) * ps).Take(ps);
            }

            return await q.ToListAsync();
        }

        public async Task<IReadOnlyList<string>> GetDistinctAccionesAsync(DateTime desdeUtc, DateTime hastaUtc)
        {
            var desde = DateTime.SpecifyKind(desdeUtc, DateTimeKind.Utc);
            var hasta = DateTime.SpecifyKind(hastaUtc, DateTimeKind.Utc);
            if (hasta <= desde) hasta = desde.AddDays(1);

            return await _context.AuditoriaEventos
                .AsNoTracking()
                .Where(e => e.Fecha >= desde && e.Fecha < hasta)
                .Select(e => e.Accion)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<string>> GetDistinctEntidadTiposAsync(DateTime desdeUtc, DateTime hastaUtc)
        {
            var desde = DateTime.SpecifyKind(desdeUtc, DateTimeKind.Utc);
            var hasta = DateTime.SpecifyKind(hastaUtc, DateTimeKind.Utc);
            if (hasta <= desde) hasta = desde.AddDays(1);

            return await _context.AuditoriaEventos
                .AsNoTracking()
                .Where(e => e.Fecha >= desde && e.Fecha < hasta && e.EntidadTipo != null && e.EntidadTipo != string.Empty)
                .Select(e => e.EntidadTipo)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }
    }
}
