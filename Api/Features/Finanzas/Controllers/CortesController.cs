using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.CasosUso.Finanzas.Cortes;
using FinancieraSoluciones.Application.DTOs.Finanzas.Cortes;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Finanzas
{
    [ApiController]
    [Route("api/finanzas/cortes")]
    [Authorize]
    public class CortesController : ControllerBase
    {
        private readonly ObtenerCortesCasoUso _obtenerCasoUso;

        public CortesController(ObtenerCortesCasoUso obtenerCasoUso)
        {
            _obtenerCasoUso = obtenerCasoUso;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CorteCajaDto>>>> Obtener(
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] int? page = null,
            [FromQuery] int? pageSize = null)
        {
            var inicio = fechaInicio?.Date ?? DateTime.Today.AddMonths(-1);
            var fin = fechaFin?.Date ?? DateTime.Today;
            var result = await _obtenerCasoUso.Ejecutar(inicio, fin, page, pageSize);
            return Ok(ApiResponse<IEnumerable<CorteCajaDto>>.Success(result));
        }
    }
}
