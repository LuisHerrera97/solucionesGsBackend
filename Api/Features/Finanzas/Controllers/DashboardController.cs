using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Api.Utils;
using FinancieraSoluciones.Application.CasosUso.Finanzas;
using FinancieraSoluciones.Application.CasosUso.Finanzas.Caja;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Finanzas
{
    [ApiController]
    [Route("api/creditos/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly ObtenerResumenDashboardCasoUso _obtenerResumenCasoUso;
        private readonly ObtenerMovimientosEnRangoCasoUso _obtenerMovimientosEnRangoCasoUso;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public DashboardController(
            ObtenerResumenDashboardCasoUso obtenerResumenCasoUso,
            ObtenerMovimientosEnRangoCasoUso obtenerMovimientosEnRangoCasoUso,
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _obtenerResumenCasoUso = obtenerResumenCasoUso;
            _obtenerMovimientosEnRangoCasoUso = obtenerMovimientosEnRangoCasoUso;
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

        [HttpGet("movimientos")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MovimientoCajaDto>>>> ObtenerMovimientos(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] Guid? zonaId = null,
            [FromQuery] Guid? cobradorId = null)
        {
            var desde = fechaDesde?.Date ?? DateTime.Today;
            var hasta = fechaHasta?.Date ?? DateTime.Today;
            var userId = CurrentUser.GetUserId(User);
            var zonaRes = await CobranzaZonaFiltroResolver.ResolverAsync(
                userId,
                zonaId,
                _usuarioRepositorio,
                _zonaCobranzaRepositorio,
                _permisoBotonRepositorio);

            if (zonaRes.TieneError)
            {
                return Ok(ApiResponse<IEnumerable<MovimientoCajaDto>>.Fail(zonaRes.MensajeError ?? string.Empty, zonaRes.CodigoError));
            }

            var result = await _obtenerMovimientosEnRangoCasoUso.Ejecutar(
                desde,
                hasta,
                page,
                pageSize,
                cobradorId,
                zonaRes.AplicarFiltroZona ? zonaRes.ZonaId : null);
            return Ok(ApiResponse<IEnumerable<MovimientoCajaDto>>.Success(result));
        }
    }
}
