using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ObtenerModulosCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IModuloRepositorio _moduloRepositorio;

        public ObtenerModulosCasoUso(IMapper mapper, IModuloRepositorio moduloRepositorio)
        {
            _mapper = mapper;
            _moduloRepositorio = moduloRepositorio;
        }

        public async Task<IEnumerable<ModuloDto>> Ejecutar(int? page = null, int? pageSize = null)
        {
            var modulos = await _moduloRepositorio.GetAllAsync(page, pageSize);
            return modulos.Select(m =>
            {
                var dto = _mapper.Map<ModuloDto>(m);
                dto.TienePermiso = false;
                return dto;
            });
        }
    }
}
