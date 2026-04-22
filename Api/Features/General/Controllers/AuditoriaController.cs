using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.CasosUso.General;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Features.General.Controllers
{
    [ApiController]
    [Route("api/general/auditoria")]
    [Authorize]
    public class AuditoriaController : ControllerBase
    {
        private readonly ObtenerAuditoriaCasoUso _obtenerAuditoriaCasoUso;
        private readonly ObtenerAuditoriaFiltrosOpcionesCasoUso _obtenerAuditoriaFiltrosOpcionesCasoUso;

        public AuditoriaController(
            ObtenerAuditoriaCasoUso obtenerAuditoriaCasoUso,
            ObtenerAuditoriaFiltrosOpcionesCasoUso obtenerAuditoriaFiltrosOpcionesCasoUso)
        {
            _obtenerAuditoriaCasoUso = obtenerAuditoriaCasoUso;
            _obtenerAuditoriaFiltrosOpcionesCasoUso = obtenerAuditoriaFiltrosOpcionesCasoUso;
        }

        [HttpGet("filtros")]
        public async Task<ActionResult<ApiResponse<AuditoriaFiltrosOpcionesDto>>> GetFiltros(
            [FromQuery] DateTime? desdeUtc,
            [FromQuery] DateTime? hastaUtc)
        {
            var desde = desdeUtc ?? DateTime.UtcNow.AddDays(-7);
            var hasta = hastaUtc ?? DateTime.UtcNow;
            var result = await _obtenerAuditoriaFiltrosOpcionesCasoUso.Ejecutar(desde, hasta);
            return Ok(ApiResponse<AuditoriaFiltrosOpcionesDto>.Success(result));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AuditoriaEventoDto>>>> Get(
            [FromQuery] DateTime? desdeUtc,
            [FromQuery] DateTime? hastaUtc,
            [FromQuery] Guid? usuarioId,
            [FromQuery] string accion,
            [FromQuery] string entidadTipo,
            [FromQuery] Guid? entidadId,
            [FromQuery] int? page,
            [FromQuery] int? pageSize)
        {
            var desde = desdeUtc ?? DateTime.UtcNow.AddDays(-7);
            var hasta = hastaUtc ?? DateTime.UtcNow;

            var result = await _obtenerAuditoriaCasoUso.Ejecutar(desde, hasta, usuarioId, accion, entidadTipo, entidadId, page, pageSize);
            return Ok(ApiResponse<IEnumerable<AuditoriaEventoDto>>.Success(result));
        }
    }
}

