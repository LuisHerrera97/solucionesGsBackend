using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ActualizarPaginaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IPaginaRepositorio _paginaRepositorio;
        private readonly IModuloRepositorio _moduloRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarPaginaCasoUso(IMapper mapper, IPaginaRepositorio paginaRepositorio, IModuloRepositorio moduloRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _paginaRepositorio = paginaRepositorio;
            _moduloRepositorio = moduloRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginaDto> Ejecutar(Guid idPagina, PaginaDto paginaDto)
        {
            var pagina = await _paginaRepositorio.GetByIdAsync(idPagina);
            if (pagina == null)
            {
                throw new ArgumentException("La página especificada no existe");
            }

            if (!string.Equals(pagina.Clave, paginaDto.Clave, StringComparison.OrdinalIgnoreCase))
            {
                var existente = await _paginaRepositorio.GetByClaveAsync(paginaDto.Clave);
                if (existente != null && existente.Id != idPagina)
                {
                    throw new ArgumentException("La clave de página ya está en uso");
                }
            }

            var modulo = await _moduloRepositorio.GetByIdAsync(paginaDto.IdModulo);
            if (modulo == null)
            {
                throw new ArgumentException("El módulo especificado no existe");
            }

            pagina.Nombre = paginaDto.Nombre;
            pagina.Clave = paginaDto.Clave;
            pagina.Ruta = paginaDto.Ruta;
            pagina.IdModulo = paginaDto.IdModulo;
            pagina.Activo = paginaDto.Activo;
            pagina.EnMenu = paginaDto.EnMenu ?? true;
            pagina.Orden = paginaDto.Orden;

            await _paginaRepositorio.UpdateAsync(pagina);
            await _unitOfWork.SaveChangesAsync();

            var respuesta = _mapper.Map<PaginaDto>(pagina);
            respuesta.NombreModulo = modulo.Nombre;
            respuesta.TienePermiso = false;
            return respuesta;
        }
    }
}
