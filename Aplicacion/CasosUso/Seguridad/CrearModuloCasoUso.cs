using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class CrearModuloCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IModuloRepositorio _moduloRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public CrearModuloCasoUso(IMapper mapper, IModuloRepositorio moduloRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _moduloRepositorio = moduloRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<ModuloDto> Ejecutar(ModuloDto moduloDto)
        {
            var existente = await _moduloRepositorio.GetByClaveAsync(moduloDto.Clave);
            if (existente != null)
            {
                throw new ArgumentException("La clave de módulo ya está en uso");
            }

            var modulo = _mapper.Map<Modulo>(moduloDto);
            modulo.Id = Guid.NewGuid();
            modulo.FechaCreacion = DateTime.UtcNow;

            await _moduloRepositorio.AddAsync(modulo);
            await _unitOfWork.SaveChangesAsync();

            var respuesta = _mapper.Map<ModuloDto>(modulo);
            respuesta.TienePermiso = false;
            return respuesta;
        }
    }
}
