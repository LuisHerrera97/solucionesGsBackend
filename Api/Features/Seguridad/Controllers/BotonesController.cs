using FinancieraSoluciones.Application.CasosUso.Seguridad;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/seguridad/botones")]
    [Authorize]
    public class BotonesController : ControllerBase
    {
        private readonly ObtenerBotonesCasoUso _obtenerBotonesCasoUso;
        private readonly CrearBotonCasoUso _crearBotonCasoUso;
        private readonly ActualizarBotonCasoUso _actualizarBotonCasoUso;
        private readonly EliminarBotonCasoUso _eliminarBotonCasoUso;

        public BotonesController(
            ObtenerBotonesCasoUso obtenerBotonesCasoUso,
            CrearBotonCasoUso crearBotonCasoUso,
            ActualizarBotonCasoUso actualizarBotonCasoUso,
            EliminarBotonCasoUso eliminarBotonCasoUso)
        {
            _obtenerBotonesCasoUso = obtenerBotonesCasoUso;
            _crearBotonCasoUso = crearBotonCasoUso;
            _actualizarBotonCasoUso = actualizarBotonCasoUso;
            _eliminarBotonCasoUso = eliminarBotonCasoUso;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<BotonDto>>>> ObtenerTodos([FromQuery] int? page, [FromQuery] int? pageSize)
        {
            var botones = await _obtenerBotonesCasoUso.Ejecutar(page, pageSize);
            return Ok(ApiResponse<IEnumerable<BotonDto>>.Success(botones));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<BotonDto>>> Crear([FromBody] BotonDto botonDto)
        {
            try
            {
                var boton = await _crearBotonCasoUso.Ejecutar(botonDto);
                return Ok(ApiResponse<BotonDto>.Success(boton, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<BotonDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{idBoton:guid}")]
        public async Task<ActionResult<ApiResponse<BotonDto>>> Actualizar(Guid idBoton, [FromBody] BotonDto botonDto)
        {
            try
            {
                var boton = await _actualizarBotonCasoUso.Ejecutar(idBoton, botonDto);
                return Ok(ApiResponse<BotonDto>.Success(boton));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<BotonDto>.Fail(ex.Message, 400));
            }
        }

        [HttpDelete("{idBoton:guid}")]
        public async Task<ActionResult<ApiResponse<bool>>> Eliminar(Guid idBoton)
        {
            var resultado = await _eliminarBotonCasoUso.Ejecutar(idBoton);
            if (!resultado)
            {
                return Ok(ApiResponse<bool>.Fail("No se encontró el botón", 404));
            }

            return Ok(ApiResponse<bool>.Success(true));
        }
    }
}
