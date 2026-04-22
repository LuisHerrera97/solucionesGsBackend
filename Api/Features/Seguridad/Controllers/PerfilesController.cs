using FinancieraSoluciones.Application.CasosUso.Seguridad;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/seguridad/perfiles")]
    [Authorize]
    public class PerfilesController : ControllerBase
    {
        private readonly CrearPerfilCasoUso _crearPerfilCasoUso;
        private readonly ActualizarPerfilCasoUso _actualizarPerfilCasoUso;
        private readonly EliminarPerfilCasoUso _eliminarPerfilCasoUso;
        private readonly ObtenerPerfilesCasoUso _obtenerPerfilesCasoUso;

        public PerfilesController(
            CrearPerfilCasoUso crearPerfilCasoUso,
            ActualizarPerfilCasoUso actualizarPerfilCasoUso,
            EliminarPerfilCasoUso eliminarPerfilCasoUso,
            ObtenerPerfilesCasoUso obtenerPerfilesCasoUso)
        {
            _crearPerfilCasoUso = crearPerfilCasoUso;
            _actualizarPerfilCasoUso = actualizarPerfilCasoUso;
            _eliminarPerfilCasoUso = eliminarPerfilCasoUso;
            _obtenerPerfilesCasoUso = obtenerPerfilesCasoUso;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PerfilDto>>>> ObtenerTodos([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var perfiles = await _obtenerPerfilesCasoUso.Ejecutar(page, pageSize);
            return Ok(ApiResponse<IEnumerable<PerfilDto>>.Success(perfiles));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PerfilDto>>> Crear([FromBody] PerfilDto perfilDto)
        {
            try
            {
                var perfil = await _crearPerfilCasoUso.Ejecutar(perfilDto);
                return Ok(ApiResponse<PerfilDto>.Success(perfil, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<PerfilDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{idPerfil:guid}")]
        public async Task<ActionResult<ApiResponse<PerfilDto>>> Actualizar(Guid idPerfil, [FromBody] PerfilDto perfilDto)
        {
            try
            {
                var perfil = await _actualizarPerfilCasoUso.Ejecutar(idPerfil, perfilDto);
                return Ok(ApiResponse<PerfilDto>.Success(perfil));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<PerfilDto>.Fail(ex.Message, 400));
            }
        }

        [HttpDelete("{idPerfil:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Eliminar(Guid idPerfil)
        {
            var resultado = await _eliminarPerfilCasoUso.Ejecutar(idPerfil);
            if (!resultado)
            {
                return Ok(ApiResponse<bool>.Fail("No se encontró el perfil", 404));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}
