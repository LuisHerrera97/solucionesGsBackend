using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.CasosUso.Cobranza.Cobranza;
using FinancieraSoluciones.Application.DTOs.Cobranza.Cobranza;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Api.Utils;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Cobranza
{
    [ApiController]
    [Route("api/cobranza/cobranza")]
    [Authorize]
    public class CobranzaController : ControllerBase
    {
        private readonly ObtenerCobranzaCasoUso _obtenerCasoUso;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public CobranzaController(
            ObtenerCobranzaCasoUso obtenerCasoUso,
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _obtenerCasoUso = obtenerCasoUso;
            _usuarioRepositorio = usuarioRepositorio;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<MovimientoCobranzaDto>>>> Obtener(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] string busqueda = null,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] Guid? zonaId = null)
        {
            var inicio = fechaInicio?.Date ?? DateTime.Today;
            var fin = fechaFin?.Date ?? DateTime.Today;
            if (fin < inicio) (inicio, fin) = (fin, inicio);

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
                return Ok(ApiResponse<IEnumerable<MovimientoCobranzaDto>>.Fail(zonaRes.MensajeError ?? string.Empty, zonaRes.CodigoError));
            }

            var result = await _obtenerCasoUso.Ejecutar(inicio, fin, busqueda, page, pageSize, zonaRes.ZonaId, zonaRes.AplicarFiltroZona);
            return Ok(ApiResponse<IEnumerable<MovimientoCobranzaDto>>.Success(result));
        }
    }
}
