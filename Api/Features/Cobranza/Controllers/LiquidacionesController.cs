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

namespace FinancieraSoluciones.Api.Controllers.Cobranza
{
    [ApiController]
    [Route("api/cobranza/liquidaciones")]
    [Authorize]
    public class LiquidacionesController : ControllerBase
    {
        private readonly ObtenerResumenLiquidacionPendienteCasoUso _obtenerResumenCasoUso;
        private readonly CrearLiquidacionCobranzaCasoUso _crearLiquidacionCasoUso;
        private readonly ObtenerLiquidacionesCobradorCasoUso _obtenerHistorialCasoUso;
        private readonly ObtenerTodasLiquidacionesCasoUso _obtenerTodasCasoUso;
        private readonly ConfirmarLiquidacionCobranzaCasoUso _confirmarCasoUso;
        private readonly RechazarLiquidacionCobranzaCasoUso _rechazarCasoUso;
        private readonly ObtenerResumenLiquidacionesCajaCasoUso _obtenerResumenCobradoresCasoUso;
        private readonly ObtenerMovimientosPendientesLiquidacionCobradorCasoUso _obtenerMovimientosPendientesCasoUso;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public LiquidacionesController(
            ObtenerResumenLiquidacionPendienteCasoUso obtenerResumenCasoUso,
            CrearLiquidacionCobranzaCasoUso crearLiquidacionCasoUso,
            ObtenerLiquidacionesCobradorCasoUso obtenerHistorialCasoUso,
            ObtenerTodasLiquidacionesCasoUso obtenerTodasCasoUso,
            ConfirmarLiquidacionCobranzaCasoUso confirmarCasoUso,
            RechazarLiquidacionCobranzaCasoUso rechazarCasoUso,
            ObtenerResumenLiquidacionesCajaCasoUso obtenerResumenCobradoresCasoUso,
            ObtenerMovimientosPendientesLiquidacionCobradorCasoUso obtenerMovimientosPendientesCasoUso,
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _obtenerResumenCasoUso = obtenerResumenCasoUso;
            _crearLiquidacionCasoUso = crearLiquidacionCasoUso;
            _obtenerHistorialCasoUso = obtenerHistorialCasoUso;
            _obtenerTodasCasoUso = obtenerTodasCasoUso;
            _confirmarCasoUso = confirmarCasoUso;
            _rechazarCasoUso = rechazarCasoUso;
            _obtenerResumenCobradoresCasoUso = obtenerResumenCobradoresCasoUso;
            _obtenerMovimientosPendientesCasoUso = obtenerMovimientosPendientesCasoUso;
            _usuarioRepositorio = usuarioRepositorio;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        [HttpGet("cobradores/resumen")]
        [RequireBotonPermiso("COBRANZA_GESTION_LIQUIDACIONES_VER")]
        public async Task<ActionResult<ApiResponse<ResumenLiquidacionesCajaDto>>> ObtenerResumenCobradores(
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

            var result = await _obtenerResumenCobradoresCasoUso.Ejecutar(
                desde,
                hasta,
                zonaRes.AplicarFiltroZona ? zonaRes.ZonaId : null,
                cobradorId);

            return Ok(ApiResponse<ResumenLiquidacionesCajaDto>.Success(result));
        }

        [HttpGet("cobradores/{cobradorId:guid}/movimientos-pendientes")]
        [RequireBotonPermiso("COBRANZA_GESTION_LIQUIDACIONES_VER")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MovimientoCajaDto>>>> ObtenerMovimientosPendientesCobrador(
            Guid cobradorId,
            [FromQuery] DateTime? fecha = null)
        {
            var dia = fecha?.Date ?? DateTime.Today;
            var result = await _obtenerMovimientosPendientesCasoUso.Ejecutar(cobradorId, dia);
            return Ok(ApiResponse<IEnumerable<MovimientoCajaDto>>.Success(result));
        }

        [HttpGet("pendiente/resumen")]
        public async Task<ActionResult<ApiResponse<LiquidacionPendienteResumenDto>>> ObtenerResumenPendiente()
        {
            var userId = CurrentUser.GetUserId(User);
            if (!userId.HasValue)
            {
                return Ok(ApiResponse<LiquidacionPendienteResumenDto>.Fail("Usuario no autenticado", 401));
            }

            var result = await _obtenerResumenCasoUso.Ejecutar(userId.Value, DateTime.Today);
            return Ok(ApiResponse<LiquidacionPendienteResumenDto>.Success(result));
        }

        [HttpGet("historial")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LiquidacionCobranzaDto>>>> ObtenerHistorial(
            [FromQuery] DateTime? fechaInicio, 
            [FromQuery] DateTime? fechaFin)
        {
            var userId = CurrentUser.GetUserId(User);
            if (!userId.HasValue)
            {
                return Ok(ApiResponse<IEnumerable<LiquidacionCobranzaDto>>.Fail("Usuario no autenticado", 401));
            }

            var inicio = fechaInicio ?? DateTime.Today.AddDays(-30);
            var fin = fechaFin ?? DateTime.Today;

            var result = await _obtenerHistorialCasoUso.Ejecutar(userId.Value, inicio, fin);
            return Ok(ApiResponse<IEnumerable<LiquidacionCobranzaDto>>.Success(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<LiquidacionCobranzaDto>>> Crear([FromBody] CrearLiquidacionCobranzaRequestDto request)
        {
            var userId = CurrentUser.GetUserId(User);
            if (!userId.HasValue)
            {
                return Ok(ApiResponse<LiquidacionCobranzaDto>.Fail("Usuario no autenticado", 401));
            }

            try
            {
                var result = await _crearLiquidacionCasoUso.Ejecutar(userId.Value, request);
                return Ok(ApiResponse<LiquidacionCobranzaDto>.Success(result, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<LiquidacionCobranzaDto>.Fail(ex.Message, 400));
            }
        }

        [HttpGet("all")]
        [RequireBotonPermiso("COBRANZA_GESTION_LIQUIDACIONES_VER")]
        public async Task<ActionResult<ApiResponse<IEnumerable<LiquidacionCobranzaDto>>>> ObtenerTodas(
            [FromQuery] DateTime? fechaInicio, 
            [FromQuery] DateTime? fechaFin,
            [FromQuery] Guid? zonaId = null)
        {
            var inicio = fechaInicio ?? DateTime.Today.AddDays(-30);
            var fin = fechaFin ?? DateTime.Today;
            var userId = CurrentUser.GetUserId(User);
            var zonaRes = await CobranzaZonaFiltroResolver.ResolverAsync(
                userId,
                zonaId,
                _usuarioRepositorio,
                _zonaCobranzaRepositorio,
                _permisoBotonRepositorio,
                CobranzaZonaFiltroResolver.PermisoCobranzaTodasZonas);

            if (zonaRes.TieneError)
            {
                return Ok(ApiResponse<IEnumerable<LiquidacionCobranzaDto>>.Fail(zonaRes.MensajeError ?? string.Empty, zonaRes.CodigoError));
            }

            var result = await _obtenerTodasCasoUso.Ejecutar(inicio, fin, zonaRes.AplicarFiltroZona ? zonaRes.ZonaId : null);
            return Ok(ApiResponse<IEnumerable<LiquidacionCobranzaDto>>.Success(result));
        }

        [HttpPost("{id:guid}/confirmar")]
        [RequireBotonPermiso("COBRANZA_GESTION_LIQUIDACIONES_CONFIRMAR")]
        public async Task<ActionResult<ApiResponse<object>>> Confirmar(Guid id)
        {
            var userId = CurrentUser.GetUserId(User);
            if (!userId.HasValue)
            {
                return Ok(ApiResponse<object>.Fail("Usuario no autenticado", 401));
            }

            try
            {
                await _confirmarCasoUso.Ejecutar(id, userId.Value);
                return Ok(ApiResponse<object>.Success(new { ok = true }));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<object>.Fail(ex.Message, 400));
            }
        }

        [HttpPost("{id:guid}/rechazar")]
        [RequireBotonPermiso("COBRANZA_GESTION_LIQUIDACIONES_RECHAZAR")]
        public async Task<ActionResult<ApiResponse<object>>> Rechazar(Guid id)
        {
            var userId = CurrentUser.GetUserId(User);
            if (!userId.HasValue)
            {
                return Ok(ApiResponse<object>.Fail("Usuario no autenticado", 401));
            }

            try
            {
                await _rechazarCasoUso.Ejecutar(id, userId.Value);
                return Ok(ApiResponse<object>.Success(new { ok = true }));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<object>.Fail(ex.Message, 400));
            }
        }
    }
}

