using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Caja
{
    public class ObtenerMovimientosEnRangoCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;
        private readonly ILiquidacionCobranzaRepositorio _liquidacionRepositorio;

        public ObtenerMovimientosEnRangoCasoUso(
            IMapper mapper,
            IMovimientoCajaRepositorio movimientoRepositorio,
            ILiquidacionCobranzaRepositorio liquidacionRepositorio)
        {
            _mapper = mapper;
            _movimientoRepositorio = movimientoRepositorio;
            _liquidacionRepositorio = liquidacionRepositorio;
        }

        public async Task<IEnumerable<MovimientoCajaDto>> Ejecutar(
            DateTime fechaDesde,
            DateTime fechaHasta,
            int? page = null,
            int? pageSize = null,
            Guid? cobradorId = null,
            Guid? zonaId = null)
        {
            var movimientos = (await _movimientoRepositorio.ObtenerEnRangoAsync(fechaDesde, fechaHasta, page, pageSize, cobradorId, zonaId)).ToList();
            var liqIds = movimientos
                .Where(m => m.LiquidacionCobranzaId.HasValue)
                .Select(m => m.LiquidacionCobranzaId!.Value)
                .Distinct()
                .ToArray();
            var estDict = await _liquidacionRepositorio.GetEstatusPorIdsAsync(liqIds);

            return movimientos.Select(m =>
            {
                var dto = _mapper.Map<MovimientoCajaDto>(m);
                dto.EstatusFichaFinanzas = MovimientoCajaEstatusFinanzasHelper.CalcularEstatusFichaFinanzas(m, estDict);
                return dto;
            });
        }
    }
}
