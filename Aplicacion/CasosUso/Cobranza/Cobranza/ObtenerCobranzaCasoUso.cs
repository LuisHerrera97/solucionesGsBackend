using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Cobranza.Cobranza;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Cobranza;

namespace FinancieraSoluciones.Application.CasosUso.Cobranza.Cobranza
{
    public class ObtenerCobranzaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly ICobranzaRepositorio _cobranzaRepositorio;

        public ObtenerCobranzaCasoUso(IMapper mapper, ICobranzaRepositorio cobranzaRepositorio)
        {
            _mapper = mapper;
            _cobranzaRepositorio = cobranzaRepositorio;
        }

        public async Task<IEnumerable<MovimientoCobranzaDto>> Ejecutar(
            DateTime fechaInicio,
            DateTime fechaFin,
            string busqueda,
            int? page = null,
            int? pageSize = null,
            Guid? zonaId = null,
            bool aplicarFiltroZona = true)
        {
            var movimientos = await _cobranzaRepositorio.ObtenerAsync(fechaInicio, fechaFin, busqueda, page, pageSize, zonaId, aplicarFiltroZona);
            return movimientos.Select(m => _mapper.Map<MovimientoCobranzaDto>(m));
        }
    }
}
