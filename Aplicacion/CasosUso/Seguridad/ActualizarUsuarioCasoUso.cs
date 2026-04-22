using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ActualizarUsuarioCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarUsuarioCasoUso(
            IMapper mapper,
            IUsuarioRepositorio usuarioRepositorio,
            IPerfilRepositorio perfilRepositorio,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _usuarioRepositorio = usuarioRepositorio;
            _perfilRepositorio = perfilRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioDto> Ejecutar(UsuarioDto usuarioDto)
        {
            var usuarioExistente = await _usuarioRepositorio.GetByIdAsync(usuarioDto.Id);
            if (usuarioExistente == null)
            {
                throw new ArgumentException("El usuario especificado no existe");
            }

            var perfil = await _perfilRepositorio.GetByIdAsync(usuarioDto.IdPerfil);
            if (perfil == null)
            {
                throw new ArgumentException("El perfil especificado no existe");
            }

            usuarioExistente.Nombre = usuarioDto.Nombre;
            usuarioExistente.ApellidoPaterno = usuarioDto.ApellidoPaterno;
            usuarioExistente.ApellidoMaterno = usuarioDto.ApellidoMaterno;
            usuarioExistente.UsuarioAcceso = usuarioDto.UsuarioAcceso;
            usuarioExistente.Activo = usuarioDto.Activo;
            usuarioExistente.IdPerfil = usuarioDto.IdPerfil;
            usuarioExistente.IdZonaCobranza = usuarioDto.IdZonaCobranza;

            await _usuarioRepositorio.UpdateAsync(usuarioExistente);
            await _unitOfWork.SaveChangesAsync();

            var usuarioRespuesta = _mapper.Map<UsuarioDto>(usuarioExistente);
            usuarioRespuesta.NombrePerfil = perfil.Nombre;

            return usuarioRespuesta;
        }
    }
}
