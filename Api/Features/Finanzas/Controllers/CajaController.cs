using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Api.Utils;
using FinancieraSoluciones.Application.CasosUso.Cobranza.Liquidaciones;
using FinancieraSoluciones.Application.CasosUso.Finanzas.Caja;
using FinancieraSoluciones.Application.DTOs.Cobranza.Liquidaciones;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Finanzas
{
    [ApiController]
    [Route("api/finanzas/caja")]
    [Authorize]
    public class CajaController : ControllerBase
    {
        private readonly ObtenerMovimientosTurnoCasoUso _obtenerTurnoCasoUso;
        private readonly ObtenerMovimientosEnRangoCasoUso _obtenerRangoCasoUso;

        private readonly LiquidarCobradorCajaCasoUso _liquidarCobradorCasoUso;
        private readonly ObtenerResumenLiquidacionesCajaCasoUso _obtenerResumenLiquidacionesCasoUso;
        private readonly RealizarCorteCasoUso _realizarCorteCasoUso;
        private readonly MarcarMovimientosRecibidoCajaCasoUso _marcarRecibidoCajaCasoUso;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public CajaController(
            ObtenerMovimientosTurnoCasoUso obtenerTurnoCasoUso,
            ObtenerMovimientosEnRangoCasoUso obtenerRangoCasoUso,

            LiquidarCobradorCajaCasoUso liquidarCobradorCasoUso,
            ObtenerResumenLiquidacionesCajaCasoUso obtenerResumenLiquidacionesCasoUso,
            RealizarCorteCasoUso realizarCorteCasoUso,
            MarcarMovimientosRecibidoCajaCasoUso marcarRecibidoCajaCasoUso,
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _obtenerTurnoCasoUso = obtenerTurnoCasoUso;
            _obtenerRangoCasoUso = obtenerRangoCasoUso;

            _liquidarCobradorCasoUso = liquidarCobradorCasoUso;
            _obtenerResumenLiquidacionesCasoUso = obtenerResumenLiquidacionesCasoUso;
            _realizarCorteCasoUso = realizarCorteCasoUso;
            _marcarRecibidoCajaCasoUso = marcarRecibidoCajaCasoUso;
            _usuarioRepositorio = usuarioRepositorio;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        [HttpGet("turno")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MovimientoCajaDto>>>> ObtenerTurno([FromQuery] DateTime? fecha = null)
        {
            var result = await _obtenerTurnoCasoUso.Ejecutar(fecha?.Date);
            return Ok(ApiResponse<IEnumerable<MovimientoCajaDto>>.Success(result));
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

            var result = await _obtenerRangoCasoUso.Ejecutar(
                desde,
                hasta,
                page,
                pageSize,
                cobradorId,
                zonaRes.AplicarFiltroZona ? zonaRes.ZonaId : null);
            return Ok(ApiResponse<IEnumerable<MovimientoCajaDto>>.Success(result));
        }

        [HttpGet("liquidaciones/resumen")]
        [RequireBotonPermiso("CAJA_REGISTRAR_MOVIMIENTO")]
        public async Task<ActionResult<ApiResponse<ResumenLiquidacionesCajaDto>>> ObtenerResumenLiquidaciones(
            [FromQuery] DateTime? fechaDesde = null,
            [FromQuery] DateTime? fechaHasta = null,
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
                return Ok(ApiResponse<ResumenLiquidacionesCajaDto>.Fail(zonaRes.MensajeError ?? string.Empty, zonaRes.CodigoError));
            }

            var result = await _obtenerResumenLiquidacionesCasoUso.Ejecutar(
                desde,
                hasta,
                zonaRes.AplicarFiltroZona ? zonaRes.ZonaId : null,
                cobradorId);

            return Ok(ApiResponse<ResumenLiquidacionesCajaDto>.Success(result));
        }

        [HttpPost("liquidaciones/cobrar")]
        [RequireBotonPermiso("CAJA_REGISTRAR_MOVIMIENTO")]
        public async Task<ActionResult<ApiResponse<LiquidacionCobranzaDto>>> CobrarLiquidacion([FromBody] LiquidarCobradorRequestDto request)
        {
            var userId = CurrentUser.GetUserId(User);
            if (!userId.HasValue)
            {
                return Ok(ApiResponse<LiquidacionCobranzaDto>.Fail("Usuario no autenticado", 401));
            }

            try
            {
                var result = await _liquidarCobradorCasoUso.Ejecutar(userId.Value, request);
                return Ok(ApiResponse<LiquidacionCobranzaDto>.Success(result, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<LiquidacionCobranzaDto>.Fail(ex.Message, 400));
            }
        }



        [HttpPost("movimientos/marcar-recibido-caja")]
        [RequireBotonPermiso("CAJA_REGISTRAR_MOVIMIENTO")]
        public async Task<ActionResult<ApiResponse<object>>> MarcarRecibidoCaja([FromBody] MarcarRecibidoCajaRequestDto request)
        {
            var userId = CurrentUser.GetUserId(User);
            if (!userId.HasValue)
            {
                return Ok(ApiResponse<object>.Fail("Usuario no autenticado", 401));
            }

            try
            {
                var count = await _marcarRecibidoCajaCasoUso.Ejecutar(userId.Value, request);
                return Ok(ApiResponse<object>.Success(new { marcados = count }));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<object>.Fail(ex.Message, 400));
            }
        }

        [HttpPost("corte")]
        [RequireBotonPermiso("CAJA_REALIZAR_CORTE")]
        public async Task<ActionResult<ApiResponse<object>>> RealizarCorte([FromBody] RealizarCorteRequestDto request)
        {
            var userId = CurrentUser.GetUserId(User);
            if (!userId.HasValue)
            {
                return Ok(ApiResponse<object>.Fail("Usuario no autenticado", 401));
            }

            try
            {
                var result = await _realizarCorteCasoUso.Ejecutar(userId.Value, request);
                return Ok(ApiResponse<object>.Success(result, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<object>.Fail(ex.Message, 400));
            }
        }

    }
}
