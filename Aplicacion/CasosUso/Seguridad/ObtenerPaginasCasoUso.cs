using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ObtenerPaginasCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IPaginaRepositorio _paginaRepositorio;

        public ObtenerPaginasCasoUso(IMapper mapper, IPaginaRepositorio paginaRepositorio)
        {
            _mapper = mapper;
            _paginaRepositorio = paginaRepositorio;
        }

        public async Task<IEnumerable<PaginaDto>> Ejecutar(int? page = null, int? pageSize = null)
        {
            var paginas = await _paginaRepositorio.GetAllAsync(page, pageSize);
            return paginas.Select(p =>
            {
                var dto = _mapper.Map<PaginaDto>(p);
                dto.NombreModulo = p.Modulo?.Nombre ?? string.Empty;
                dto.TienePermiso = false;
                return dto;
            });
        }
    }
}
