using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Cobranza.Pendientes;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Pendientes;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.Cobranza.Pendientes
{
    public class PendientesRepositorio : IPendientesRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PendientesRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<PendienteCobro> Items, int TotalCount)> ObtenerAsync(
            DateTime hoy,
            string? busqueda,
            Guid? zonaId,
            bool aplicarFiltroZona,
            int page,
            int pageSize)
        {
            var hoyDate = hoy.Date;

            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize < 1 ? 25 : pageSize;
            if (normalizedPageSize > 100) normalizedPageSize = 100;

            var query = _context.Fichas
                .AsNoTracking()
                .Where(f => !f.Pagada && f.Fecha <= hoyDate && f.Credito != null && f.Credito.Cliente != null)
                .Select(f => new
                {
                    Ficha = f,
                    Credito = f.Credito!,
                    Cliente = f.Credito!.Cliente!,
                    ClienteNombre = f.Credito!.Cliente!.Nombre + " " + f.Credito!.Cliente!.Apellido,
                    EsVencida = f.Fecha < hoyDate,
                });

            if (aplicarFiltroZona)
            {
                if (zonaId.HasValue)
                {
                    query = query.Where(x => x.Cliente.IdZona == zonaId.Value);
                }
                else
                {
                    query = query.Where(x => false);
                }
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                var pattern = LikePattern(busqueda);
                query = query.Where(x =>
                    EF.Functions.ILike(x.Cliente.Nombre, pattern) ||
                    EF.Functions.ILike(x.Cliente.Apellido, pattern) ||
                    EF.Functions.ILike(x.Cliente.Nombre + " " + x.Cliente.Apellido, pattern) ||
                    EF.Functions.ILike(x.Cliente.Apellido + " " + x.Cliente.Nombre, pattern) ||
                    (x.Cliente.Negocio != null && EF.Functions.ILike(x.Cliente.Negocio, pattern)) ||
                    EF.Functions.ILike(x.Credito.Folio, pattern));
            }

            query = query
                .OrderByDescending(x => x.EsVencida)
                .ThenBy(x => x.Ficha.Fecha)
                .ThenBy(x => x.ClienteNombre)
                .ThenBy(x => x.Ficha.Num);

            var totalCount = await query.CountAsync();

            var skip = (normalizedPage - 1) * normalizedPageSize;

            var items = await query
                .Skip(skip)
                .Take(normalizedPageSize)
                .Select(x => new PendienteCobro
                {
                    CreditoId = x.Ficha.CreditoId,
                    CreditoFolio = x.Credito.Folio ?? string.Empty,
                    NumFicha = x.Ficha.Num,
                    FechaLimite = x.Ficha.Fecha,
                    TotalFicha = x.Ficha.Capital + x.Ficha.Interes + x.Ficha.MoraAcumulada,
                    AbonoAcumulado = x.Ficha.AbonoAcumulado,
                    Pendiente = x.Ficha.SaldoPendiente,
                    Estado = x.EsVencida ? "Vencida" : "Hoy",
                    ClienteNombre = x.ClienteNombre,
                    ClienteNegocio = x.Cliente.Negocio ?? string.Empty,
                    TipoCredito = x.Credito.Tipo ?? string.Empty,
                })
                .ToListAsync();

            return (items, totalCount);
        }

        private static string LikePattern(string raw)
        {
            var t = raw.Trim().Replace("%", string.Empty, StringComparison.Ordinal).Replace("_", string.Empty, StringComparison.Ordinal);
            return $"%{t}%";
        }
    }
}
