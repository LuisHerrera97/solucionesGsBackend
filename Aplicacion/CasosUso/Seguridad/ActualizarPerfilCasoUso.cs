using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ActualizarPerfilCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarPerfilCasoUso(IMapper mapper, IPerfilRepositorio perfilRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _perfilRepositorio = perfilRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<PerfilDto> Ejecutar(Guid idPerfil, PerfilDto perfilDto)
        {
            var perfil = await _perfilRepositorio.GetByIdAsync(idPerfil);
            if (perfil == null)
            {
                throw new ArgumentException("El perfil especificado no existe");
            }

            if (!string.Equals(perfil.Clave, perfilDto.Clave, StringComparison.OrdinalIgnoreCase))
            {
                var existente = await _perfilRepositorio.GetByClaveAsync(perfilDto.Clave);
                if (existente != null && existente.Id != idPerfil)
                {
                    throw new ArgumentException("La clave de perfil ya está en uso");
                }
            }

            perfil.Nombre = perfilDto.Nombre;
            perfil.Clave = perfilDto.Clave;
            perfil.Activo = perfilDto.Activo;
            perfil.Orden = perfilDto.Orden;

            await _perfilRepositorio.UpdateAsync(perfil);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PerfilDto>(perfil);
        }
    }
}
