using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Caja
{
    public class ObtenerMovimientosEnRangoCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;

        public ObtenerMovimientosEnRangoCasoUso(
            IMapper mapper,
            IMovimientoCajaRepositorio movimientoRepositorio)
        {
            _mapper = mapper;
            _movimientoRepositorio = movimientoRepositorio;
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

            return movimientos.Select(m =>
            {
                var dto = _mapper.Map<MovimientoCajaDto>(m);
                dto.EstatusFichaFinanzas = MovimientoCajaEstatusFinanzasHelper.CalcularEstatusFichaFinanzas(m, null);
                return dto;
            });
        }
    }
}
