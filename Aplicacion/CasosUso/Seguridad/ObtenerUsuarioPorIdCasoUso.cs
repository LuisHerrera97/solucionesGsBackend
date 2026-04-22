using System;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ObtenerUsuarioPorIdCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPerfilRepositorio _perfilRepositorio;

        public ObtenerUsuarioPorIdCasoUso(
            IMapper mapper,
            IUsuarioRepositorio usuarioRepositorio,
            IPerfilRepositorio perfilRepositorio)
        {
            _mapper = mapper;
            _usuarioRepositorio = usuarioRepositorio;
            _perfilRepositorio = perfilRepositorio;
        }

        public async Task<UsuarioDto> Ejecutar(Guid id)
        {
            var usuario = await _usuarioRepositorio.GetByIdAsync(id);
            
            if (usuario == null)
            {
                return null;
            }

            var perfil = await _perfilRepositorio.GetByIdAsync(usuario.IdPerfil);

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            usuarioDto.NombrePerfil = perfil?.Nombre ?? string.Empty;

            return usuarioDto;
        }
    }
}
