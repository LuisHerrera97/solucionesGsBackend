using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.CasosUso.Cobranza.Pendientes;
using FinancieraSoluciones.Application.DTOs.Cobranza.Pendientes;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Api.Utils;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Cobranza
{
    [ApiController]
    [Route("api/cobranza/pendientes")]
    [Authorize]
    public class PendientesController : ControllerBase
    {
        private readonly ObtenerPendientesCasoUso _obtenerCasoUso;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;
        private readonly IClock _clock;

        public PendientesController(
            ObtenerPendientesCasoUso obtenerCasoUso,
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio,
            IClock clock)
        {
            _obtenerCasoUso = obtenerCasoUso;
            _usuarioRepositorio = usuarioRepositorio;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
            _clock = clock;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PendientesListadoDto>>> Obtener(
            [FromQuery] string? busqueda = null,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] Guid? zonaId = null)
        {
            var userId = CurrentUser.GetUserId(User);
            var zonaRes = await CobranzaZonaFiltroResolver.ResolverAsync(
                userId,
                zonaId,
                _usuarioRepositorio,
                _zonaCobranzaRepositorio,
                _permisoBotonRepositorio,
                CobranzaZonaFiltroResolver.PermisoPendientesTodasZonas);

            if (zonaRes.TieneError)
            {
                return Ok(ApiResponse<PendientesListadoDto>.Fail(zonaRes.MensajeError ?? string.Empty, zonaRes.CodigoError));
            }

            var result = await _obtenerCasoUso.Ejecutar(_clock.Today, busqueda, zonaRes.ZonaId, zonaRes.AplicarFiltroZona, page, pageSize);
            return Ok(ApiResponse<PendientesListadoDto>.Success(result));
        }
    }
}
