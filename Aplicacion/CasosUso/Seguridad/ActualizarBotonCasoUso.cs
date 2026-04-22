using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ActualizarBotonCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IBotonRepositorio _botonRepositorio;
        private readonly IPaginaRepositorio _paginaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarBotonCasoUso(
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

        public async Task<BotonDto> Ejecutar(Guid idBoton, BotonDto botonDto)
        {
            var boton = await _botonRepositorio.GetByIdAsync(idBoton);
            if (boton == null)
            {
                throw new ArgumentException("El botón especificado no existe");
            }

            if (!string.Equals(boton.Clave, botonDto.Clave, StringComparison.OrdinalIgnoreCase))
            {
                var existente = await _botonRepositorio.GetByClaveAsync(botonDto.Clave);
                if (existente != null && existente.Id != idBoton)
                {
                    throw new ArgumentException("La clave de botón ya está en uso");
                }
            }

            var pagina = await _paginaRepositorio.GetByIdAsync(botonDto.IdPagina);
            if (pagina == null)
            {
                throw new ArgumentException("La página especificada no existe");
            }

            boton.Nombre = botonDto.Nombre;
            boton.Clave = botonDto.Clave;
            boton.IdPagina = botonDto.IdPagina;
            boton.Activo = botonDto.Activo;
            boton.Orden = botonDto.Orden;

            await _botonRepositorio.UpdateAsync(boton);
            await _unitOfWork.SaveChangesAsync();

            var respuesta = _mapper.Map<BotonDto>(boton);
            respuesta.NombrePagina = pagina.Nombre;
            respuesta.TienePermiso = false;
            return respuesta;
        }
    }
}
