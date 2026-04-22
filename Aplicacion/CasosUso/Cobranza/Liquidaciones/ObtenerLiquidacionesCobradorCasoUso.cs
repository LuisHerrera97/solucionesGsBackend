using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;

namespace FinancieraSoluciones.Application.CasosUso.Cobranza.Liquidaciones
{
    public class ObtenerLiquidacionesCobradorCasoUso
    {
        private readonly ILiquidacionCobranzaRepositorio _liquidacionRepositorio;
        private readonly IMapper _mapper;

        public ObtenerLiquidacionesCobradorCasoUso(
            ILiquidacionCobranzaRepositorio liquidacionRepositorio,
            IMapper mapper)
        {
            _liquidacionRepositorio = liquidacionRepositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LiquidacionCobranzaDto>> Ejecutar(Guid cobradorId, DateTime fechaInicio, DateTime fechaFin)
        {
            var liquidaciones = await _liquidacionRepositorio.GetByCobradorAsync(cobradorId, fechaInicio, fechaFin);
            return _mapper.Map<IEnumerable<LiquidacionCobranzaDto>>(liquidaciones);
        }
    }
}
