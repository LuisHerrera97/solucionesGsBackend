using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.CasosUso.General.Feriados;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Features.General.Controllers
{
    [ApiController]
    [Route("api/general/feriados")]
    [Authorize]
    public class FeriadosController : ControllerBase
    {
        private readonly ObtenerFeriadosCasoUso _obtenerCasoUso;
        private readonly CrearFeriadoCasoUso _crearCasoUso;
        private readonly ActualizarFeriadoCasoUso _actualizarCasoUso;
        private readonly EliminarFeriadoCasoUso _eliminarCasoUso;

        public FeriadosController(
            ObtenerFeriadosCasoUso obtenerCasoUso,
            CrearFeriadoCasoUso crearCasoUso,
            ActualizarFeriadoCasoUso actualizarCasoUso,
            EliminarFeriadoCasoUso eliminarCasoUso)
        {
            _obtenerCasoUso = obtenerCasoUso;
            _crearCasoUso = crearCasoUso;
            _actualizarCasoUso = actualizarCasoUso;
            _eliminarCasoUso = eliminarCasoUso;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<FeriadoDto>>>> GetAll()
        {
            var result = await _obtenerCasoUso.Ejecutar();
            return Ok(ApiResponse<IEnumerable<FeriadoDto>>.Success(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<FeriadoDto>>> Crear([FromBody] FeriadoDto dto)
        {
            try
            {
                var created = await _crearCasoUso.Ejecutar(dto);
                return Ok(ApiResponse<FeriadoDto>.Success(created, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<FeriadoDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<FeriadoDto>>> Actualizar(Guid id, [FromBody] FeriadoDto dto)
        {
            try
            {
                var updated = await _actualizarCasoUso.Ejecutar(id, dto);
                return Ok(ApiResponse<FeriadoDto>.Success(updated));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<FeriadoDto>.Fail(ex.Message, 400));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> Eliminar(Guid id)
        {
            await _eliminarCasoUso.Ejecutar(id);
            return Ok(ApiResponse<object>.Success(new { ok = true }));
        }
    }
}

