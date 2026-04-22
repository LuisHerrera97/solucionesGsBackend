using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Api.Utils;
using FinancieraSoluciones.Application.CasosUso.General;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.General
{
    [ApiController]
    [Route("api/general/configuracion")]
    [Authorize]
    public class ConfiguracionSistemaController : ControllerBase
    {
        private readonly ObtenerConfiguracionSistemaCasoUso _obtenerCasoUso;
        private readonly ActualizarConfiguracionSistemaCasoUso _actualizarCasoUso;

        public ConfiguracionSistemaController(
            ObtenerConfiguracionSistemaCasoUso obtenerCasoUso,
            ActualizarConfiguracionSistemaCasoUso actualizarCasoUso)
        {
            _obtenerCasoUso = obtenerCasoUso;
            _actualizarCasoUso = actualizarCasoUso;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ConfiguracionSistemaDto>>> Obtener()
        {
            var config = await _obtenerCasoUso.Ejecutar();
            return Ok(ApiResponse<ConfiguracionSistemaDto>.Success(config));
        }

        [HttpPut]
        [RequireBotonPermiso("CONFIGURACION_EDITAR")]
        public async Task<ActionResult<ApiResponse<ConfiguracionSistemaDto>>> Actualizar([FromBody] ConfiguracionSistemaDto dto)
        {
            try
            {
                var config = await _actualizarCasoUso.Ejecutar(dto);
                return Ok(ApiResponse<ConfiguracionSistemaDto>.Success(config));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<ConfiguracionSistemaDto>.Fail(ex.Message, 400));
            }
        }
    }
}
