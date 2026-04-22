using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.General
{
    public class ObtenerAuditoriaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public ObtenerAuditoriaCasoUso(
            IMapper mapper,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IUsuarioRepositorio usuarioRepositorio)
        {
            _mapper = mapper;
            _auditoriaRepositorio = auditoriaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<IEnumerable<AuditoriaEventoDto>> Ejecutar(DateTime desdeUtc, DateTime hastaUtc, Guid? usuarioId, string accion, string entidadTipo, Guid? entidadId, int? page, int? pageSize)
        {
            var eventos = (await _auditoriaRepositorio.GetAsync(desdeUtc, hastaUtc, usuarioId, accion, entidadTipo, entidadId, page, pageSize)).ToList();
            var idsUsuario = eventos.Where(e => e.UsuarioId.HasValue).Select(e => e.UsuarioId!.Value).Distinct().ToList();
            var usuarios = await _usuarioRepositorio.GetByIdsAsync(idsUsuario);
            var nombresPorId = usuarios.ToDictionary(u => u.Id, FormatearNombreUsuario);

            return eventos.Select(e =>
            {
                var dto = _mapper.Map<AuditoriaEventoDto>(e);
                if (e.UsuarioId.HasValue && nombresPorId.TryGetValue(e.UsuarioId.Value, out var nombre))
                    dto.UsuarioNombre = nombre;
                return dto;
            }).ToList();
        }

        private static string FormatearNombreUsuario(Usuario u)
        {
            if (u == null) return string.Empty;
            var partes = string.Join(" ", new[] { u.Nombre, u.ApellidoPaterno }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
            if (string.IsNullOrEmpty(partes)) return u.UsuarioAcceso ?? string.Empty;
            return string.IsNullOrWhiteSpace(u.UsuarioAcceso) ? partes : $"{partes} ({u.UsuarioAcceso})";
        }
    }
}
