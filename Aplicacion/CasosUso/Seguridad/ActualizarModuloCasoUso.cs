using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ActualizarModuloCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IModuloRepositorio _moduloRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarModuloCasoUso(IMapper mapper, IModuloRepositorio moduloRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _moduloRepositorio = moduloRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<ModuloDto> Ejecutar(Guid idModulo, ModuloDto moduloDto)
        {
            var modulo = await _moduloRepositorio.GetByIdAsync(idModulo);
            if (modulo == null)
            {
                throw new ArgumentException("El módulo especificado no existe");
            }

            if (!string.Equals(modulo.Clave, moduloDto.Clave, StringComparison.OrdinalIgnoreCase))
            {
                var existente = await _moduloRepositorio.GetByClaveAsync(moduloDto.Clave);
                if (existente != null && existente.Id != idModulo)
                {
                    throw new ArgumentException("La clave de módulo ya está en uso");
                }
            }

            modulo.Nombre = moduloDto.Nombre;
            modulo.Clave = moduloDto.Clave;
            modulo.Icono = moduloDto.Icono;
            modulo.Activo = moduloDto.Activo;
            modulo.Orden = moduloDto.Orden;

            await _moduloRepositorio.UpdateAsync(modulo);
            await _unitOfWork.SaveChangesAsync();

            var respuesta = _mapper.Map<ModuloDto>(modulo);
            respuesta.TienePermiso = false;
            return respuesta;
        }
    }
}
