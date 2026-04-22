using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Common;
using FinancieraSoluciones.Domain.Entidades.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.Servicios.Finanzas
{
    public class CreditoZonaAutorizacionService : ICreditoZonaAutorizacionService
    {
        private const string PermisoTodasZonas = "CREDITO_TODAS_ZONAS";

        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public CreditoZonaAutorizacionService(
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        public async Task AsegurarPuedeOperarAsync(Credito credito, Guid? usuarioId)
        {
            if (!usuarioId.HasValue) return;

            var usuario = await _usuarioRepositorio.GetByIdAsync(usuarioId.Value);
            if (usuario == null) return;

            if (await _permisoBotonRepositorio.HasPermisoAsync(usuario.IdPerfil, PermisoTodasZonas))
                return;

            if (usuario.IdZonaCobranza is not { } zonaIdUsuario) return;

            if (credito.Cliente != null && credito.Cliente.IdZona != zonaIdUsuario)
            {
                throw new ForbiddenException("No tienes permiso para cobrar este crédito (Zona no coincide)");
            }
        }
    }
}
