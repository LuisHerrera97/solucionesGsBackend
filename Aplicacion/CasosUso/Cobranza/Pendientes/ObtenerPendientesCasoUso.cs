using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Cobranza.Pendientes;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Pendientes;

namespace FinancieraSoluciones.Application.CasosUso.Cobranza.Pendientes
{
    public class ObtenerPendientesCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IPendientesRepositorio _pendientesRepositorio;

        public ObtenerPendientesCasoUso(IMapper mapper, IPendientesRepositorio pendientesRepositorio)
        {
            _mapper = mapper;
            _pendientesRepositorio = pendientesRepositorio;
        }

        public async Task<PendientesListadoDto> Ejecutar(
            DateTime hoy,
            string? busqueda,
            Guid? zonaId,
            bool aplicarFiltroZona,
            int? page,
            int? pageSize)
        {
            var p = page.GetValueOrDefault(1);
            var ps = pageSize.GetValueOrDefault(25);
            var (items, totalCount) = await _pendientesRepositorio.ObtenerAsync(hoy, busqueda, zonaId, aplicarFiltroZona, p, ps);
            return new PendientesListadoDto
            {
                Items = items.Select(x => _mapper.Map<PendienteCobroDto>(x)).ToList(),
                TotalCount = totalCount
            };
        }
    }
}
