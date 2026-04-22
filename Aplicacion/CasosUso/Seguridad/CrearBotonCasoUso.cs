using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class CrearBotonCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IBotonRepositorio _botonRepositorio;
        private readonly IPaginaRepositorio _paginaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public CrearBotonCasoUso(
            IMapper mapper,
            IBotonRepositorio botonRepositorio,
            IPaginaRepositorio paginaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _botonRepositorio = botonRepositorio;
            _paginaRepositorio = paginaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<BotonDto> Ejecutar(BotonDto botonDto)
        {
            var existente = await _botonRepositorio.GetByClaveAsync(botonDto.Clave);
            if (existente != null)
            {
                throw new ArgumentException("La clave de botón ya está en uso");
            }

            var pagina = await _paginaRepositorio.GetByIdAsync(botonDto.IdPagina);
            if (pagina == null)
            {
                throw new ArgumentException("La página especificada no existe");
            }

            var boton = _mapper.Map<Boton>(botonDto);
            boton.Id = Guid.NewGuid();
            boton.FechaCreacion = DateTime.UtcNow;

            await _botonRepositorio.AddAsync(boton);
            await _unitOfWork.SaveChangesAsync();

            var respuesta = _mapper.Map<BotonDto>(boton);
            respuesta.NombrePagina = pagina.Nombre;
            respuesta.TienePermiso = false;
            return respuesta;
        }
    }
}
