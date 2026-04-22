using FinancieraSoluciones.Application.CasosUso.Seguridad;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/seguridad/paginas")]
    [Authorize]
    public class PaginasController : ControllerBase
    {
        private readonly ObtenerPaginasCasoUso _obtenerPaginasCasoUso;
        private readonly CrearPaginaCasoUso _crearPaginaCasoUso;
        private readonly ActualizarPaginaCasoUso _actualizarPaginaCasoUso;
        private readonly EliminarPaginaCasoUso _eliminarPaginaCasoUso;

        public PaginasController(
            ObtenerPaginasCasoUso obtenerPaginasCasoUso,
            CrearPaginaCasoUso crearPaginaCasoUso,
            ActualizarPaginaCasoUso actualizarPaginaCasoUso,
            EliminarPaginaCasoUso eliminarPaginaCasoUso)
        {
            _obtenerPaginasCasoUso = obtenerPaginasCasoUso;
            _crearPaginaCasoUso = crearPaginaCasoUso;
            _actualizarPaginaCasoUso = actualizarPaginaCasoUso;
            _eliminarPaginaCasoUso = eliminarPaginaCasoUso;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PaginaDto>>>> ObtenerTodos([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var paginas = await _obtenerPaginasCasoUso.Ejecutar(page, pageSize);
            return Ok(ApiResponse<IEnumerable<PaginaDto>>.Success(paginas));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PaginaDto>>> Crear([FromBody] PaginaDto paginaDto)
        {
            try
            {
                var pagina = await _crearPaginaCasoUso.Ejecutar(paginaDto);
                return Ok(ApiResponse<PaginaDto>.Success(pagina, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<PaginaDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{idPagina:guid}")]
        public async Task<ActionResult<ApiResponse<PaginaDto>>> Actualizar(Guid idPagina, [FromBody] PaginaDto paginaDto)
        {
            try
            {
                var pagina = await _actualizarPaginaCasoUso.Ejecutar(idPagina, paginaDto);
                return Ok(ApiResponse<PaginaDto>.Success(pagina));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<PaginaDto>.Fail(ex.Message, 400));
            }
        }

        [HttpDelete("{idPagina:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Eliminar(Guid idPagina)
        {
            var resultado = await _eliminarPaginaCasoUso.Ejecutar(idPagina);
            if (!resultado)
            {
                return Ok(ApiResponse<bool>.Fail("No se encontró la página", 404));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}
