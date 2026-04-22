using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ObtenerPerfilesCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IPerfilRepositorio _perfilRepositorio;

        public ObtenerPerfilesCasoUso(IMapper mapper, IPerfilRepositorio perfilRepositorio)
        {
            _mapper = mapper;
            _perfilRepositorio = perfilRepositorio;
        }

        public async Task<IEnumerable<PerfilDto>> Ejecutar(int? page = null, int? pageSize = null)
        {
            var perfiles = await _perfilRepositorio.GetAllAsync(page, pageSize);

            var perfilesDto = perfiles.Select(p => _mapper.Map<PerfilDto>(p));

            return perfilesDto;
        }
    }
}
