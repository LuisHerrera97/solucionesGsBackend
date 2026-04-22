using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General
{
    public class ObtenerZonasCobranzaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IZonaCobranzaRepositorio _zonaRepositorio;

        public ObtenerZonasCobranzaCasoUso(IMapper mapper, IZonaCobranzaRepositorio zonaRepositorio)
        {
            _mapper = mapper;
            _zonaRepositorio = zonaRepositorio;
        }

        public async Task<IEnumerable<ZonaCobranzaDto>> Ejecutar(int? page = null, int? pageSize = null)
        {
            var zonas = await _zonaRepositorio.GetAllAsync(page, pageSize);
            return zonas.Select(z => _mapper.Map<ZonaCobranzaDto>(z));
        }
    }
}
