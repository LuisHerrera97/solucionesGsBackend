using FinancieraSoluciones.Application.CasosUso.Seguridad;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/seguridad/perfiles/{idPerfil:guid}")]
    [Authorize]
    public class PermisosController : ControllerBase
    {
        private readonly ObtenerModulosConPermisosCasoUso _obtenerModulosConPermisosCasoUso;
        private readonly ObtenerPermisosPorPerfilCasoUso _obtenerPermisosPorPerfilCasoUso;
        private readonly AsignarPermisosCasoUso _asignarPermisosCasoUso;

        public PermisosController(
            ObtenerModulosConPermisosCasoUso obtenerModulosConPermisosCasoUso,
            ObtenerPermisosPorPerfilCasoUso obtenerPermisosPorPerfilCasoUso,
            AsignarPermisosCasoUso asignarPermisosCasoUso)
        {
            _obtenerModulosConPermisosCasoUso = obtenerModulosConPermisosCasoUso;
            _obtenerPermisosPorPerfilCasoUso = obtenerPermisosPorPerfilCasoUso;
            _asignarPermisosCasoUso = asignarPermisosCasoUso;
        }

        [HttpGet("menu")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ModuloDto>>>> ObtenerMenu(Guid idPerfil)
        {
            var modulos = await _obtenerModulosConPermisosCasoUso.Ejecutar(idPerfil);
            return Ok(ApiResponse<IEnumerable<ModuloDto>>.Success(modulos));
        }

        [HttpGet("permisos")]
        public async Task<ActionResult<ApiResponse<AsignarPermisosRequestDto>>> ObtenerPermisos(Guid idPerfil)
        {
            var permisos = await _obtenerPermisosPorPerfilCasoUso.Ejecutar(idPerfil);
            return Ok(ApiResponse<AsignarPermisosRequestDto>.Success(permisos));
        }

        [HttpPost("permisos")]
        public async Task<ActionResult<ApiResponse<bool>>> GuardarPermisos(Guid idPerfil, [FromBody] AsignarPermisosRequestDto request)
        {
            if (request.IdPerfil != idPerfil)
            {
                return Ok(ApiResponse<bool>.Fail("IdPerfil no coincide con la ruta", 400));
            }

            var resultado = await _asignarPermisosCasoUso.Ejecutar(request);
            if (!resultado)
            {
                return Ok(ApiResponse<bool>.Fail("No fue posible guardar los permisos", 400));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}
