using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class CrearPerfilCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public CrearPerfilCasoUso(IMapper mapper, IPerfilRepositorio perfilRepositorio, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _perfilRepositorio = perfilRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<PerfilDto> Ejecutar(PerfilDto perfilDto)
        {
            var perfilExistente = await _perfilRepositorio.GetByClaveAsync(perfilDto.Clave);
            if (perfilExistente != null)
            {
                throw new ArgumentException("La clave de perfil ya está en uso");
            }

            var perfil = _mapper.Map<Perfil>(perfilDto);
            perfil.Id = Guid.NewGuid();
            perfil.FechaCreacion = DateTime.UtcNow;

            var perfilGuardado = await _perfilRepositorio.AddAsync(perfil);
            await _unitOfWork.SaveChangesAsync();

            var perfilRespuesta = _mapper.Map<PerfilDto>(perfilGuardado);

            return perfilRespuesta;
        }
    }
}
