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
    public class FeriadoRepositorio : IFeriadoRepositorio
    {
        private readonly ApplicationDbContext _context;

        public FeriadoRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feriado>> GetAllAsync()
        {
            return await _context.Feriados.AsNoTracking().OrderByDescending(f => f.Fecha).ToListAsync();
        }

        public async Task<IEnumerable<Feriado>> GetActivosEnRangoAsync(DateTime desde, DateTime hasta)
        {
            var d = desde.Date;
            var h = hasta.Date;
            return await _context.Feriados.AsNoTracking()
                .Where(f => f.Activo && f.Fecha >= d && f.Fecha <= h)
                .ToListAsync();
        }

        public async Task<Feriado> AddAsync(Feriado feriado)
        {
            await _context.Feriados.AddAsync(feriado);
            return feriado;
        }

        public async Task<Feriado> GetByIdAsync(Guid id)
        {
            return await _context.Feriados.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Feriado> GetByFechaAsync(DateTime fecha)
        {
            var d = fecha.Date;
            return await _context.Feriados.FirstOrDefaultAsync(f => f.Fecha == d);
        }

        public async Task UpdateAsync(Feriado feriado)
        {
            _context.Feriados.Update(feriado);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Feriados.FirstOrDefaultAsync(f => f.Id == id);
            if (entity != null) _context.Feriados.Remove(entity);
        }
    }
}
