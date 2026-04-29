using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.CasosUso.Finanzas;
using FinancieraSoluciones.Application.CasosUso.Finanzas.Caja;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Domain.Common;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using FinancieraSoluciones.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Finanzas
{
    [ApiController]
    [Route("api/creditos/creditos")]
    [Authorize]
    public class CreditosController : ControllerBase
    {
        private readonly ObtenerCreditosCasoUso _obtenerCasoUso;
        private readonly ObtenerCreditoPorIdCasoUso _obtenerPorIdCasoUso;
        private readonly CrearCreditoCasoUso _crearCasoUso;
        private readonly AbonarFichaCasoUso _abonarFichaCasoUso;
        private readonly PenalizarFichaManualCasoUso _penalizarFichaManualCasoUso;
        private readonly ReversarOperacionMovimientoCasoUso _reversarOperacionMovimientoCasoUso;
        private readonly ReestructurarCreditoCasoUso _reestructurarCasoUso;
        private readonly CondonarInteresFichaCasoUso _condonarInteresCasoUso;
        private readonly CondonarInteresMontoCasoUso _condonarInteresMontoCasoUso;
        private readonly ActualizarObservacionCasoUso _actualizarObservacionCasoUso;
        private readonly ActualizarMoraAcumuladaCasoUso _actualizarMoraAcumuladaCasoUso;
        private readonly ObtenerMovimientosPorCreditoCasoUso _obtenerMovimientosCasoUso;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;
        private readonly IPerfilRepositorio _perfilRepositorio;

        public CreditosController(
            ObtenerCreditosCasoUso obtenerCasoUso,
            ObtenerCreditoPorIdCasoUso obtenerPorIdCasoUso,
            CrearCreditoCasoUso crearCasoUso,
            AbonarFichaCasoUso abonarFichaCasoUso,
            PenalizarFichaManualCasoUso penalizarFichaManualCasoUso,
            ReversarOperacionMovimientoCasoUso reversarOperacionMovimientoCasoUso,
            ReestructurarCreditoCasoUso reestructurarCasoUso,
            CondonarInteresFichaCasoUso condonarInteresCasoUso,
            CondonarInteresMontoCasoUso condonarInteresMontoCasoUso,
            ActualizarObservacionCasoUso actualizarObservacionCasoUso,
            ActualizarMoraAcumuladaCasoUso actualizarMoraAcumuladaCasoUso,
            ObtenerMovimientosPorCreditoCasoUso obtenerMovimientosCasoUso,
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio,
            IPerfilRepositorio perfilRepositorio)
        {
            _obtenerCasoUso = obtenerCasoUso;
            _obtenerPorIdCasoUso = obtenerPorIdCasoUso;
            _crearCasoUso = crearCasoUso;
            _abonarFichaCasoUso = abonarFichaCasoUso;
            _penalizarFichaManualCasoUso = penalizarFichaManualCasoUso;
            _reversarOperacionMovimientoCasoUso = reversarOperacionMovimientoCasoUso;
            _reestructurarCasoUso = reestructurarCasoUso;
            _condonarInteresCasoUso = condonarInteresCasoUso;
            _condonarInteresMontoCasoUso = condonarInteresMontoCasoUso;
            _actualizarObservacionCasoUso = actualizarObservacionCasoUso;
            _actualizarMoraAcumuladaCasoUso = actualizarMoraAcumuladaCasoUso;
            _obtenerMovimientosCasoUso = obtenerMovimientosCasoUso;
            _usuarioRepositorio = usuarioRepositorio;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
            _perfilRepositorio = perfilRepositorio;
        }

        private async Task<ActionResult<ApiResponse<CreditoDto>>?> ValidarPermisoAbonarFichaAsync(
            Guid perfilId,
            CreditoDto creditoDto,
            int numeroFicha,
            AbonarFichaRequestDto request)
        {
            var perfil = await _perfilRepositorio.GetByIdAsync(perfilId);
            if (perfil != null && string.Equals(perfil.Clave, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var ficha = creditoDto.Fichas?.FirstOrDefault(f => f.Num == numeroFicha);
            if (ficha == null) return Ok(ApiResponse<CreditoDto>.Fail("No existe la ficha", 404));
            if (ficha.Pagada) return Ok(ApiResponse<CreditoDto>.Fail("La ficha ya está pagada", 400));

            var adeudoFicha = ficha.Capital + ficha.Interes + ficha.MoraAcumulada;
            var faltanteTotal = Math.Max(0, adeudoFicha - ficha.AbonoAcumulado);
            var montoTotalRecibido = request?.MontoAbono ?? faltanteTotal;
            if (montoTotalRecibido < 0) return Ok(ApiResponse<CreditoDto>.Fail("El monto no puede ser negativo", 400));

            var esPagoCompleto = Math.Abs(montoTotalRecibido - faltanteTotal) <= DecimalTolerance.Centavo;

            if (esPagoCompleto)
            {
                var ok = await _permisoBotonRepositorio.HasPermisoAlgunoAsync(perfilId, "CREDITO_PAGAR_FICHA", "CREDITO_ABONAR_FICHA");
                if (!ok) return Ok(ApiResponse<CreditoDto>.Fail("No tienes permisos para registrar el pago de la ficha", 403));
            }
            else
            {
                var ok = await _permisoBotonRepositorio.HasPermisoAsync(perfilId, "CREDITO_ABONAR_FICHA");
                if (!ok) return Ok(ApiResponse<CreditoDto>.Fail("No tienes permisos para abonar en esta ficha", 403));
            }

            return null;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CreditoDto>>>> ObtenerTodos(
            [FromQuery] string? searchTerm,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] Guid? zonaId)
        {
            var userId = CurrentUser.GetUserId(User);
            var zonaRes = await CobranzaZonaFiltroResolver.ResolverAsync(
                userId,
                zonaId,
                _usuarioRepositorio,
                _zonaCobranzaRepositorio,
                _permisoBotonRepositorio,
                "CREDITO_LISTA_TODAS_ZONAS");

            if (zonaRes.TieneError)
            {
                return Ok(ApiResponse<IEnumerable<CreditoDto>>.Fail(zonaRes.MensajeError ?? string.Empty, zonaRes.CodigoError));
            }

            var creditos = await _obtenerCasoUso.Ejecutar(searchTerm, page, pageSize, zonaRes.ZonaId, zonaRes.AplicarFiltroZona);
            return Ok(ApiResponse<IEnumerable<CreditoDto>>.Success(creditos));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<CreditoDto>>> ObtenerPorId(Guid id)
        {
            var credito = await _obtenerPorIdCasoUso.Ejecutar(id);
            if (credito == null) return Ok(ApiResponse<CreditoDto>.Fail("No se encontró el crédito", 404));
            return Ok(ApiResponse<CreditoDto>.Success(credito));
        }

        [HttpGet("{id:guid}/movimientos")]
        public async Task<ActionResult<ApiResponse<IEnumerable<MovimientoCajaDto>>>> ObtenerMovimientos(Guid id)
        {
            var movimientos = await _obtenerMovimientosCasoUso.Ejecutar(id);
            return Ok(ApiResponse<IEnumerable<MovimientoCajaDto>>.Success(movimientos));
        }

        [HttpPost]
        [RequireBotonPermiso("CREDITO_CREAR")]
        public async Task<ActionResult<ApiResponse<CreditoDto>>> Crear([FromBody] CrearCreditoRequestDto request)
        {
            try
            {
                var userId = CurrentUser.GetUserId(User);
                var credito = await _crearCasoUso.Ejecutar(request, userId);
                return Ok(ApiResponse<CreditoDto>.Success(credito, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<CreditoDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPost("{creditoId:guid}/fichas/{numeroFicha:int}/abonos")]
        public async Task<ActionResult<ApiResponse<CreditoDto>>> AbonarFicha(Guid creditoId, int numeroFicha, [FromBody] AbonarFichaRequestDto request)
        {
            var perfilId = CurrentUser.GetPerfilId(User);
            if (!perfilId.HasValue) return Ok(ApiResponse<CreditoDto>.Fail("Usuario no autenticado", 401));

            var creditoPrevio = await _obtenerPorIdCasoUso.Ejecutar(creditoId);
            if (creditoPrevio == null) return Ok(ApiResponse<CreditoDto>.Fail("No se encontró el crédito", 404));

            var permisoRes = await ValidarPermisoAbonarFichaAsync(perfilId.Value, creditoPrevio, numeroFicha, request);
            if (permisoRes != null) return permisoRes;

            var userId = CurrentUser.GetUserId(User);
            var credito = await _abonarFichaCasoUso.Ejecutar(creditoId, numeroFicha, request, userId);
            return Ok(ApiResponse<CreditoDto>.Success(credito));
        }

        [HttpPost("{creditoId:guid}/fichas/vigentes/abonos")]
        [RequireAnyBotonPermiso("CREDITO_PAGAR_FICHA", "CREDITO_ABONAR_FICHA")]
        public async Task<ActionResult<ApiResponse<CreditoDto>>> AbonarFichasVigentes(Guid creditoId, [FromBody] AbonarFichaRequestDto request)
        {
            var userId = CurrentUser.GetUserId(User);
            var credito = await _abonarFichaCasoUso.EjecutarMultiplesVigentes(creditoId, request, userId);
            return Ok(ApiResponse<CreditoDto>.Success(credito));
        }

        [HttpPost("{creditoId:guid}/fichas/{numeroFicha:int}/multas")]
        [RequireAnyBotonPermiso("CREDITO_PENALIZAR_FICHA", "CREDITO_ABONAR_FICHA")]
        public async Task<ActionResult<ApiResponse<CreditoDto>>> PenalizarFicha(Guid creditoId, int numeroFicha, [FromBody] PenalizarFichaRequestDto request)
        {
            var userId = CurrentUser.GetUserId(User);
            var credito = await _penalizarFichaManualCasoUso.Ejecutar(creditoId, numeroFicha, request, userId);
            return Ok(ApiResponse<CreditoDto>.Success(credito));
        }

        [HttpPost("{creditoId:guid}/movimientos/{movimientoId:guid}/reversa")]
        [RequireAnyBotonPermiso("CREDITO_PAGAR_FICHA", "CREDITO_ABONAR_FICHA")]
        public async Task<ActionResult<ApiResponse<CreditoDto>>> ReversarOperacion(Guid creditoId, Guid movimientoId)
        {
            var userId = CurrentUser.GetUserId(User);
            var credito = await _reversarOperacionMovimientoCasoUso.Ejecutar(creditoId, movimientoId, userId);
            return Ok(ApiResponse<CreditoDto>.Success(credito));
        }

        [HttpPut("{creditoId:guid}/reestructura")]
        [RequireBotonPermiso("CREDITO_REESTRUCTURAR")]
        public async Task<ActionResult<ApiResponse<CreditoDto>>> Reestructurar(Guid creditoId, [FromBody] ReestructurarCreditoRequestDto request)
        {
            try
            {
                var userId = CurrentUser.GetUserId(User);
                var credito = await _reestructurarCasoUso.Ejecutar(creditoId, request, userId);
                return Ok(ApiResponse<CreditoDto>.Success(credito));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<CreditoDto>.Fail(ex.Message, 400));
            }
            catch (InvalidOperationException ex)
            {
                return Ok(ApiResponse<CreditoDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPost("{creditoId:guid}/fichas/{numeroFicha:int}/condonacion")]
        [RequireBotonPermiso("CREDITO_CONDONAR_INTERES")]
        public async Task<ActionResult<ApiResponse<string>>> CondonarInteres(Guid creditoId, int numeroFicha)
        {
            try
            {
                var userId = CurrentUser.GetUserId(User);
                await _condonarInteresCasoUso.Ejecutar(creditoId, numeroFicha, userId);
                return Ok(ApiResponse<string>.Success("Interés condonado correctamente"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message, 400));
            }
        }
        [HttpPost("{creditoId:guid}/condonacion-monto")]
        [RequireBotonPermiso("CREDITO_CONDONAR_INTERES")]
        public async Task<ActionResult<ApiResponse<string>>> CondonarInteresMonto(Guid creditoId, [FromBody] CondonarInteresMontoRequestDto request)
        {
            try
            {
                var userId = CurrentUser.GetUserId(User);
                await _condonarInteresMontoCasoUso.Ejecutar(creditoId, request.Monto, userId);
                return Ok(ApiResponse<string>.Success("Interés global condonado correctamente"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{creditoId:guid}/observacion")]
        [RequireBotonPermiso("CREDITO_REESTRUCTURAR")] // o algun permiso general de credito
        public async Task<ActionResult<ApiResponse<string>>> ActualizarObservacion(Guid creditoId, [FromBody] ActualizarObservacionRequestDto request)
        {
            try
            {
                await _actualizarObservacionCasoUso.Ejecutar(creditoId, request.Observacion);
                return Ok(ApiResponse<string>.Success("Observación actualizada correctamente"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<string>.Fail(ex.Message, 400));
            }
        }

        [HttpPost("aplicar-mora")]
        [RequireBotonPermiso("CREDITO_REESTRUCTURAR")] // o algun permiso general
        public async Task<ActionResult<ApiResponse<int>>> AplicarMora()
        {
            try
            {
                var hoy = DateTime.Today;
                var updated = await _actualizarMoraAcumuladaCasoUso.Ejecutar(hoy);
                return Ok(ApiResponse<int>.Success(updated, $"Se actualizaron {updated} fichas con mora."));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<int>.Fail(ex.Message, 400));
            }
        }
    }
}
