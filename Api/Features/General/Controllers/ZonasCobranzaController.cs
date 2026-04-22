using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.CasosUso.General;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.General
{
    [ApiController]
    [Route("api/general/zonas")]
    [Authorize]
    public class ZonasCobranzaController : ControllerBase
    {
        private readonly ObtenerZonasCobranzaCasoUso _obtenerCasoUso;
        private readonly CrearZonaCobranzaCasoUso _crearCasoUso;
        private readonly ActualizarZonaCobranzaCasoUso _actualizarCasoUso;
        private readonly EliminarZonaCobranzaCasoUso _eliminarCasoUso;

        public ZonasCobranzaController(
            ObtenerZonasCobranzaCasoUso obtenerCasoUso,
            CrearZonaCobranzaCasoUso crearCasoUso,
            ActualizarZonaCobranzaCasoUso actualizarCasoUso,
            EliminarZonaCobranzaCasoUso eliminarCasoUso)
        {
            _obtenerCasoUso = obtenerCasoUso;
            _crearCasoUso = crearCasoUso;
            _actualizarCasoUso = actualizarCasoUso;
            _eliminarCasoUso = eliminarCasoUso;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ZonaCobranzaDto>>>> ObtenerTodos([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var zonas = await _obtenerCasoUso.Ejecutar(page, pageSize);
            return Ok(ApiResponse<IEnumerable<ZonaCobranzaDto>>.Success(zonas));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ZonaCobranzaDto>>> Crear([FromBody] ZonaCobranzaDto dto)
        {
            try
            {
                var zona = await _crearCasoUso.Ejecutar(dto);
                return Ok(ApiResponse<ZonaCobranzaDto>.Success(zona, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<ZonaCobranzaDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<ZonaCobranzaDto>>> Actualizar(Guid id, [FromBody] ZonaCobranzaDto dto)
        {
            try
            {
                var zona = await _actualizarCasoUso.Ejecutar(id, dto);
                return Ok(ApiResponse<ZonaCobranzaDto>.Success(zona));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<ZonaCobranzaDto>.Fail(ex.Message, 400));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Eliminar(Guid id)
        {
            var ok = await _eliminarCasoUso.Ejecutar(id);
            if (!ok) return Ok(ApiResponse<bool>.Fail("No se encontró la zona", 404));
            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}
