using System;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;

namespace FinancieraSoluciones.Application.CasosUso.Cobranza.Liquidaciones
{
    public class ObtenerResumenLiquidacionPendienteCasoUso
    {
        private readonly IMovimientoCajaRepositorio _movimientoCajaRepositorio;

        public ObtenerResumenLiquidacionPendienteCasoUso(IMovimientoCajaRepositorio movimientoCajaRepositorio)
        {
            _movimientoCajaRepositorio = movimientoCajaRepositorio;
        }

        public async Task<LiquidacionPendienteResumenDto> Ejecutar(Guid cobradorId, DateTime fecha)
        {
            var movimientos = await _movimientoCajaRepositorio.ObtenerPendientesLiquidacionAsync(cobradorId, fecha);
            var list = movimientos.ToList();

            static decimal MontoTarjeta(MovimientoCaja m) =>
                string.Equals(m.Medio, "Tarjeta", StringComparison.OrdinalIgnoreCase) ? m.Total : 0m;

            var totalEfectivo = list.Sum(m => m.MontoEfectivo ?? 0);
            var totalTarjeta = list.Sum(MontoTarjeta);
            var totalTransferencia = list.Sum(m => m.MontoTransferencia ?? 0);
            var total = list.Sum(m => m.Total);

            return new LiquidacionPendienteResumenDto
            {
                CantidadMovimientos = list.Count,
                TotalEfectivo = totalEfectivo,
                TotalTarjeta = totalTarjeta,
                TotalTransferencia = totalTransferencia,
                Total = total
            };
        }
    }
}

