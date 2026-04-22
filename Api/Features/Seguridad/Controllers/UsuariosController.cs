using FinancieraSoluciones.Application.CasosUso.Seguridad;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/seguridad/usuarios")]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly CrearUsuarioCasoUso _crearUsuarioCasoUso;
        private readonly ObtenerUsuariosCasoUso _obtenerUsuariosCasoUso;
        private readonly ObtenerUsuarioPorIdCasoUso _obtenerUsuarioPorIdCasoUso;
        private readonly ActualizarUsuarioCasoUso _actualizarUsuarioCasoUso;
        private readonly EliminarUsuarioCasoUso _eliminarUsuarioCasoUso;
        private readonly ResetPasswordAdminCasoUso _resetPasswordAdminCasoUso;

        public UsuariosController(
            CrearUsuarioCasoUso crearUsuarioCasoUso,
            ObtenerUsuariosCasoUso obtenerUsuariosCasoUso,
            ObtenerUsuarioPorIdCasoUso obtenerUsuarioPorIdCasoUso,
            ActualizarUsuarioCasoUso actualizarUsuarioCasoUso,
            EliminarUsuarioCasoUso eliminarUsuarioCasoUso,
            ResetPasswordAdminCasoUso resetPasswordAdminCasoUso)
        {
            _crearUsuarioCasoUso = crearUsuarioCasoUso;
            _obtenerUsuariosCasoUso = obtenerUsuariosCasoUso;
            _obtenerUsuarioPorIdCasoUso = obtenerUsuarioPorIdCasoUso;
            _actualizarUsuarioCasoUso = actualizarUsuarioCasoUso;
            _eliminarUsuarioCasoUso = eliminarUsuarioCasoUso;
            _resetPasswordAdminCasoUso = resetPasswordAdminCasoUso;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UsuarioDto>>>> ObtenerTodos([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var usuarios = await _obtenerUsuariosCasoUso.Ejecutar(page, pageSize);
            return Ok(ApiResponse<IEnumerable<UsuarioDto>>.Success(usuarios));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> ObtenerPorId(Guid id)
        {
            var usuario = await _obtenerUsuarioPorIdCasoUso.Ejecutar(id);

            if (usuario == null)
            {
                return Ok(ApiResponse<UsuarioDto>.Fail("No se encontró el usuario", 404));
            }

            return Ok(ApiResponse<UsuarioDto>.Success(usuario));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> Crear([FromBody] UsuarioCrearDto usuarioDto)
        {
            try
            {
                var usuario = await _crearUsuarioCasoUso.Ejecutar(usuarioDto);
                return Ok(ApiResponse<UsuarioDto>.Success(usuario, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<UsuarioDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> Actualizar(Guid id, [FromBody] UsuarioDto usuarioDto)
        {
            try
            {
                usuarioDto.Id = id;
                var usuario = await _actualizarUsuarioCasoUso.Ejecutar(usuarioDto);
                return Ok(ApiResponse<UsuarioDto>.Success(usuario));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<UsuarioDto>.Fail(ex.Message, 400));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Eliminar(Guid id)
        {
            var resultado = await _eliminarUsuarioCasoUso.Ejecutar(id);

            if (!resultado)
            {
                return Ok(ApiResponse<bool>.Fail("No se encontró el usuario", 404));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }

        [HttpPost("{id}/reset-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword(Guid id, [FromBody] ResetPasswordAdminRequestDto request)
        {
            try
            {
                var adminId = Guid.Parse(User.Claims.First(c => c.Type == "id").Value);
                await _resetPasswordAdminCasoUso.Ejecutar(id, adminId, request);
                return Ok(ApiResponse<bool>.Success(true));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<bool>.Fail(ex.Message, 400));
            }
        }
    }
}
