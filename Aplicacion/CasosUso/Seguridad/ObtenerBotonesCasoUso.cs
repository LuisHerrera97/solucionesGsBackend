using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ObtenerBotonesCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IBotonRepositorio _botonRepositorio;

        public ObtenerBotonesCasoUso(IMapper mapper, IBotonRepositorio botonRepositorio)
        {
            _mapper = mapper;
            _botonRepositorio = botonRepositorio;
        }

        public async Task<IEnumerable<BotonDto>> Ejecutar(int? page = null, int? pageSize = null)
        {
            var botones = await _botonRepositorio.GetAllAsync(page, pageSize);
            return botones.Select(b =>
            {
                var dto = _mapper.Map<BotonDto>(b);
                dto.NombrePagina = b.Pagina?.Nombre ?? string.Empty;
                dto.TienePermiso = false;
                return dto;
            });
        }
    }
}
