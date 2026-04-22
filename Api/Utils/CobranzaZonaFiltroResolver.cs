using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Api.Utils
{
    public sealed class CobranzaZonaFiltroResult
    {
        public bool TieneError { get; private init; }
        public string? MensajeError { get; private init; }
        public int CodigoError { get; private init; }
        public Guid? ZonaId { get; private init; }
        public string? ZonaNombre { get; private init; }
        public bool AplicarFiltroZona { get; private init; }

        public static CobranzaZonaFiltroResult Exito(Guid? zonaId, string? zonaNombre, bool aplicarFiltroZona) =>
            new CobranzaZonaFiltroResult
            {
                TieneError = false,
                ZonaId = zonaId,
                ZonaNombre = zonaNombre,
                AplicarFiltroZona = aplicarFiltroZona,
            };

        public static CobranzaZonaFiltroResult Error(string mensaje, int codigo) =>
            new CobranzaZonaFiltroResult
            {
                TieneError = true,
                MensajeError = mensaje,
                CodigoError = codigo,
                ZonaId = null,
                ZonaNombre = null,
                AplicarFiltroZona = true,
            };
    }

    public static class CobranzaZonaFiltroResolver
    {
        public const string PermisoClienteTodasZonas = "CLIENTE_TODAS_ZONAS";
        public const string PermisoCreditoTodasZonas = "CREDITO_LISTA_TODAS_ZONAS";
        public const string PermisoPendientesTodasZonas = "PENDIENTES_TODAS_ZONAS";
        public const string PermisoCobranzaTodasZonas = "COBRANZA_TODAS_ZONAS";

        public static async Task<CobranzaZonaFiltroResult> ResolverAsync(
            Guid? userId,
            Guid? zonaQueryId,
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio,
            string permissionName = PermisoCobranzaTodasZonas)
        {
            Guid? zonaId = null;
            string? zonaNombre = null;
            var aplicarFiltroZona = true;

            if (!userId.HasValue)
            {
                return CobranzaZonaFiltroResult.Exito(null, null, true);
            }

            var usuario = await usuarioRepositorio.GetByIdAsync(userId.Value);
            var puedeElegirZona = usuario != null && await permisoBotonRepositorio.HasPermisoAsync(usuario.IdPerfil, permissionName);

            if (puedeElegirZona)
            {
                if (zonaQueryId.HasValue)
                {
                    var coincidencia = await zonaCobranzaRepositorio.GetByIdAsync(zonaQueryId.Value);
                    if (coincidencia == null || string.IsNullOrWhiteSpace(coincidencia.Nombre))
                    {
                        return CobranzaZonaFiltroResult.Error("La zona indicada no existe en el catálogo.", 400);
                    }

                    zonaId = coincidencia.Id;
                    zonaNombre = coincidencia.Nombre.Trim();
                    aplicarFiltroZona = true;
                }
                else
                {
                    aplicarFiltroZona = false;
                }
            }
            else if (zonaQueryId.HasValue)
            {
                return CobranzaZonaFiltroResult.Error("No tienes permiso para filtrar por zona.", 403);
            }
            else if (usuario?.IdZonaCobranza.HasValue == true)
            {
                var zonaCobranza = await zonaCobranzaRepositorio.GetByIdAsync(usuario.IdZonaCobranza.Value);
                zonaId = zonaCobranza?.Id;
                zonaNombre = zonaCobranza?.Nombre?.Trim();
            }

            return CobranzaZonaFiltroResult.Exito(zonaId, zonaNombre, aplicarFiltroZona);
        }
    }
}
