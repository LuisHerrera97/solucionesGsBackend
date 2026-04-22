using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class CrearPaginaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IPaginaRepositorio _paginaRepositorio;
        private readonly IModuloRepositorio _moduloRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public CrearPaginaCasoUso(IMapper mapper, IPaginaRepositorio paginaRepositorio, IModuloRepositorio moduloRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _paginaRepositorio = paginaRepositorio;
            _moduloRepositorio = moduloRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginaDto> Ejecutar(PaginaDto paginaDto)
        {
            var existente = await _paginaRepositorio.GetByClaveAsync(paginaDto.Clave);
            if (existente != null)
            {
                throw new ArgumentException("La clave de página ya está en uso");
            }

            var modulo = await _moduloRepositorio.GetByIdAsync(paginaDto.IdModulo);
            if (modulo == null)
            {
                throw new ArgumentException("El módulo especificado no existe");
            }

            var pagina = _mapper.Map<Pagina>(paginaDto);
            pagina.Id = Guid.NewGuid();
            pagina.FechaCreacion = DateTime.UtcNow;

            await _paginaRepositorio.AddAsync(pagina);
            await _unitOfWork.SaveChangesAsync();

            var respuesta = _mapper.Map<PaginaDto>(pagina);
            respuesta.NombreModulo = modulo.Nombre;
            respuesta.TienePermiso = false;
            return respuesta;
        }
    }
}
