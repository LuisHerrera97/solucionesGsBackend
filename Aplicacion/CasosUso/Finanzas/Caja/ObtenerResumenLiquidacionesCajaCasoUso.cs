using System;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.Finanzas.Caja;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Caja
{
    public class ObtenerResumenLiquidacionesCajaCasoUso
    {
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ILiquidacionCobranzaRepositorio _liquidacionRepositorio;

        public ObtenerResumenLiquidacionesCajaCasoUso(
            IMovimientoCajaRepositorio movimientoRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            ILiquidacionCobranzaRepositorio liquidacionRepositorio)
        {
            _movimientoRepositorio = movimientoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _liquidacionRepositorio = liquidacionRepositorio;
        }

        public async Task<ResumenLiquidacionesCajaDto> Ejecutar(DateTime fechaDesde, DateTime fechaHasta, Guid? zonaId = null, Guid? cobradorId = null)
        {
            var tipoFicha = TipoMovimientoCaja.Ficha.ToStoredString();
            var tipoIngreso = TipoMovimientoCaja.Ingreso.ToStoredString();
            var movimientos = (await _movimientoRepositorio.ObtenerEnRangoAsync(
                fechaDesde,
                fechaHasta,
                null,
                null,
                cobradorId,
                zonaId))
                .Where(m =>
                    m.CobradorId.HasValue &&
                    (string.Equals(m.Tipo, tipoFicha, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(m.Tipo, tipoIngreso, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var liqIds = movimientos
                .Where(m => m.LiquidacionCobranzaId.HasValue)
                .Select(m => m.LiquidacionCobranzaId!.Value)
                .Distinct()
                .ToArray();
            var estatusPorLiq = await _liquidacionRepositorio.GetEstatusPorIdsAsync(liqIds);

            var cobradorIds = movimientos.Select(m => m.CobradorId!.Value).Distinct().ToArray();
            var usuarios = await _usuarioRepositorio.GetByIdsAsync(cobradorIds);
            var nombres = usuarios.ToDictionary(
                u => u.Id,
                u => $"{u.Nombre} {u.ApellidoPaterno}".Trim());

            static decimal MontoTarjeta(MovimientoCaja m) =>
                string.Equals(m.Medio, "Tarjeta", StringComparison.OrdinalIgnoreCase) ? m.Total : 0m;

            var cobradores = movimientos
                .GroupBy(m => m.CobradorId!.Value)
                .Select(g => new CobradorLiquidacionResumenDto
                {
                    CobradorId = g.Key,
                    NombreCobrador = nombres.TryGetValue(g.Key, out var nombre) ? nombre : g.Key.ToString(),
                    CantidadMovimientos = g.Count(),
                    Total = g.Sum(x => x.Total),
                    TotalEfectivo = g.Sum(x => x.MontoEfectivo ?? 0),
                    TotalTarjeta = g.Sum(MontoTarjeta),
                    TotalTransferencia = g.Sum(x => x.MontoTransferencia ?? 0),
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            var estados = new ResumenEstadoFichaCajaDto
            {
                Total = movimientos.Sum(m => m.Total),
                Pendiente = movimientos
                    .Where(m => !MovimientoCajaEstatusFinanzasHelper.EsCobradoParaResumen(m, estatusPorLiq) && !m.CorteCajaId.HasValue)
                    .Sum(m => m.Total),
                Liquidado = movimientos
                    .Where(m => MovimientoCajaEstatusFinanzasHelper.EsCobradoParaResumen(m, estatusPorLiq) && !m.CorteCajaId.HasValue)
                    .Sum(m => m.Total),
                EnCorte = movimientos.Where(m => m.CorteCajaId.HasValue).Sum(m => m.Total),
            };

            return new ResumenLiquidacionesCajaDto
            {
                Cobradores = cobradores,
                Estados = estados,
            };
        }
    }
}
