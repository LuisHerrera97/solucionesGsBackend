using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Cobranza.Cobranza;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Cobranza;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.Cobranza.Cobranza
{
    public class CobranzaRepositorio : ICobranzaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public CobranzaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovimientoCobranza>> ObtenerAsync(
            DateTime fechaInicio,
            DateTime fechaFin,
            string busqueda,
            int? page = null,
            int? pageSize = null,
            Guid? zonaId = null,
            bool aplicarFiltroZona = true)
        {
            var inicio = fechaInicio.Date;
            var finExclusive = fechaFin.Date.AddDays(1);
            var tipoFicha = TipoMovimientoCaja.Ficha.ToStoredString();

            var query = _context.MovimientosCaja
                .AsNoTracking()
                .Include(m => m.Credito)
                .ThenInclude(c => c.Cliente)
                .Where(m => m.Tipo == tipoFicha)
                .Where(m => m.Fecha >= inicio && m.Fecha < finExclusive)
                .AsQueryable();

            if (aplicarFiltroZona)
            {
                if (zonaId.HasValue)
                {
                    query = query.Where(m =>
                        m.Credito != null &&
                        m.Credito.Cliente != null &&
                        m.Credito.Cliente.IdZona == zonaId.Value);
                }
                else
                {
                    query = query.Where(m => false);
                }
            }

            var b = busqueda?.Trim();
            if (string.IsNullOrWhiteSpace(b) && (page.HasValue || pageSize.HasValue))
            {
                var normalizedPage = page.GetValueOrDefault(1);
                if (normalizedPage < 1) normalizedPage = 1;

                var normalizedPageSize = pageSize.GetValueOrDefault(100);
                if (normalizedPageSize < 1) normalizedPageSize = 1;
                if (normalizedPageSize > 500) normalizedPageSize = 500;

                var skip = (normalizedPage - 1) * normalizedPageSize;
                query = query
                    .OrderByDescending(m => m.Fecha)
                    .ThenByDescending(m => m.Hora)
                    .Skip(skip)
                    .Take(normalizedPageSize);
            }

            var movimientos = await query.ToListAsync();

            var list = movimientos.Select(m =>
            {
                var credito = m.Credito;
                var cliente = credito?.Cliente;
                return new MovimientoCobranza
                {
                    CreditoId = m.CreditoId ?? Guid.Empty,
                    CreditoFolio = credito?.Folio ?? string.Empty,
                    NumFicha = m.NumeroFicha ?? 0,
                    FechaPago = m.Fecha,
                    HoraPago = m.Hora ?? string.Empty,
                    FechaLimite = m.Fecha, // En movimientos no tenemos la fecha límite original de la ficha fácilmente sin otro join
                    Capital = 0, // No guardamos capital vs interés en el movimiento individual
                    Interes = 0,
                    Abono = m.Abono ?? 0,
                    Mora = m.Mora ?? 0,
                    TotalCobrado = m.Total,
                    ClienteNombre = cliente != null ? $"{cliente.Nombre} {cliente.Apellido}" : string.Empty,
                    ClienteNegocio = cliente?.Negocio ?? string.Empty,
                    TipoCredito = credito?.Tipo ?? string.Empty
                };
            });

            if (!string.IsNullOrWhiteSpace(b))
            {
                var bn = b.ToLowerInvariant();
                list = list.Where(x =>
                {
                    var texto = $"{x.ClienteNombre} {x.ClienteNegocio} {x.CreditoFolio} {x.CreditoId}".ToLowerInvariant();
                    return texto.Contains(bn);
                });
            }

            var ordered = list
                .OrderByDescending(x => x.FechaPago)
                .ThenByDescending(x => x.HoraPago)
                .AsEnumerable();

            if ((page.HasValue || pageSize.HasValue) && !string.IsNullOrWhiteSpace(b))
            {
                var normalizedPage = page.GetValueOrDefault(1);
                if (normalizedPage < 1) normalizedPage = 1;

                var normalizedPageSize = pageSize.GetValueOrDefault(100);
                if (normalizedPageSize < 1) normalizedPageSize = 1;
                if (normalizedPageSize > 500) normalizedPageSize = 500;

                var skip = (normalizedPage - 1) * normalizedPageSize;
                ordered = ordered.Skip(skip).Take(normalizedPageSize);
            }

            return ordered.ToList();
        }
    }
}
