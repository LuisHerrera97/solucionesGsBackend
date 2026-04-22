using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;

namespace FinancieraSoluciones.Application.CasosUso.Cobranza.Liquidaciones
{
    public class ObtenerMovimientosPendientesLiquidacionCobradorCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;
        private readonly ILiquidacionCobranzaRepositorio _liquidacionRepositorio;

        public ObtenerMovimientosPendientesLiquidacionCobradorCasoUso(
            IMapper mapper,
            IMovimientoCajaRepositorio movimientoRepositorio,
            ILiquidacionCobranzaRepositorio liquidacionRepositorio)
        {
            _mapper = mapper;
            _movimientoRepositorio = movimientoRepositorio;
            _liquidacionRepositorio = liquidacionRepositorio;
        }

        public async Task<IEnumerable<MovimientoCajaDto>> Ejecutar(Guid cobradorId, DateTime fecha)
        {
            var movimientos = (await _movimientoRepositorio.ObtenerPendientesLiquidacionAsync(cobradorId, fecha)).ToList();
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
