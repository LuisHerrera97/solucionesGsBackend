using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ObtenerUsuariosCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;

        public ObtenerUsuariosCasoUso(
            IMapper mapper,
            IUsuarioRepositorio usuarioRepositorio,
            IPerfilRepositorio perfilRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio)
        {
            _mapper = mapper;
            _usuarioRepositorio = usuarioRepositorio;
            _perfilRepositorio = perfilRepositorio;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
        }

        public async Task<IEnumerable<UsuarioDto>> Ejecutar(int? page = null, int? pageSize = null)
        {
            var usuarios = await _usuarioRepositorio.GetAllAsync(page, pageSize);
            
            var perfiles = await _perfilRepositorio.GetAllAsync();
            var perfilesDict = perfiles.ToDictionary(p => p.Id, p => p.Nombre);

            var zonas = await _zonaCobranzaRepositorio.GetAllAsync();
            var zonasDict = zonas.ToDictionary(z => z.Id, z => z.Nombre);

            var usuariosDto = usuarios.Select(u =>
            {
                var dto = _mapper.Map<UsuarioDto>(u);
                dto.NombrePerfil = perfilesDict.TryGetValue(u.IdPerfil, out var nombre) ? nombre : string.Empty;
                dto.NombreZonaCobranza =
                    u.IdZonaCobranza.HasValue && zonasDict.TryGetValue(u.IdZonaCobranza.Value, out var zonaNombre)
                        ? zonaNombre
                        : string.Empty;
                return dto;
            });

            return usuariosDto;
        }
    }
}
