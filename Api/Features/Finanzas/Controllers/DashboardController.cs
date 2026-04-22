using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Api.Utils;
using FinancieraSoluciones.Application.CasosUso.Finanzas;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Finanzas
{
    [ApiController]
    [Route("api/finanzas/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ObtenerResumenDashboardCasoUso _obtenerResumenCasoUso;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public DashboardController(
            ObtenerResumenDashboardCasoUso obtenerResumenCasoUso,
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _obtenerResumenCasoUso = obtenerResumenCasoUso;
            _usuarioRepositorio = usuarioRepositorio;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        [HttpGet("resumen")]
        public async Task<ActionResult<ApiResponse<DashboardResumenDto>>> ObtenerResumen([FromQuery] Guid? zonaId)
        {
            var userId = CurrentUser.GetUserId(User);
            var zonaRes = await CobranzaZonaFiltroResolver.ResolverAsync(
                userId,
                zonaId,
                _usuarioRepositorio,
                _zonaCobranzaRepositorio,
                _permisoBotonRepositorio,
                "CLIENTE_TODAS_ZONAS");

            if (zonaRes.TieneError)
            {
                return Ok(ApiResponse<DashboardResumenDto>.Fail(zonaRes.MensajeError ?? string.Empty, zonaRes.CodigoError));
            }

            var resumen = await _obtenerResumenCasoUso.Ejecutar(zonaRes.ZonaId, zonaRes.AplicarFiltroZona);
            return Ok(ApiResponse<DashboardResumenDto>.Success(resumen));
        }
    }
}
