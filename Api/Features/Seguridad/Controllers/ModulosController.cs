using FinancieraSoluciones.Application.CasosUso.Seguridad;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/seguridad/modulos")]
    [Authorize]
    public class ModulosController : ControllerBase
    {
        private readonly ObtenerModulosCasoUso _obtenerModulosCasoUso;
        private readonly CrearModuloCasoUso _crearModuloCasoUso;
        private readonly ActualizarModuloCasoUso _actualizarModuloCasoUso;
        private readonly EliminarModuloCasoUso _eliminarModuloCasoUso;

        public ModulosController(
            ObtenerModulosCasoUso obtenerModulosCasoUso,
            CrearModuloCasoUso crearModuloCasoUso,
            ActualizarModuloCasoUso actualizarModuloCasoUso,
            EliminarModuloCasoUso eliminarModuloCasoUso)
        {
            _obtenerModulosCasoUso = obtenerModulosCasoUso;
            _crearModuloCasoUso = crearModuloCasoUso;
            _actualizarModuloCasoUso = actualizarModuloCasoUso;
            _eliminarModuloCasoUso = eliminarModuloCasoUso;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ModuloDto>>>> ObtenerTodos([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var modulos = await _obtenerModulosCasoUso.Ejecutar(page, pageSize);
            return Ok(ApiResponse<IEnumerable<ModuloDto>>.Success(modulos));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ModuloDto>>> Crear([FromBody] ModuloDto moduloDto)
        {
            try
            {
                var modulo = await _crearModuloCasoUso.Ejecutar(moduloDto);
                return Ok(ApiResponse<ModuloDto>.Success(modulo, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<ModuloDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{idModulo:guid}")]
        public async Task<ActionResult<ApiResponse<ModuloDto>>> Actualizar(Guid idModulo, [FromBody] ModuloDto moduloDto)
        {
            try
            {
                var modulo = await _actualizarModuloCasoUso.Ejecutar(idModulo, moduloDto);
                return Ok(ApiResponse<ModuloDto>.Success(modulo));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<ModuloDto>.Fail(ex.Message, 400));
            }
        }

        [HttpDelete("{idModulo:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Eliminar(Guid idModulo)
        {
            var resultado = await _eliminarModuloCasoUso.Ejecutar(idModulo);
            if (!resultado)
            {
                return Ok(ApiResponse<bool>.Fail("No se encontró el módulo", 404));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}
