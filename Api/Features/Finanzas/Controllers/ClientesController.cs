using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Api.Utils;
using FinancieraSoluciones.Application.CasosUso.Finanzas;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Finanzas
{
    [ApiController]
    [Route("api/finanzas/clientes")]
    [Authorize]
    public class ClientesController : ControllerBase
    {
        private readonly ObtenerClientesCasoUso _obtenerCasoUso;
        private readonly CrearClienteCasoUso _crearCasoUso;
        private readonly ActualizarClienteCasoUso _actualizarCasoUso;
        private readonly EliminarClienteCasoUso _eliminarCasoUso;
        private readonly ObtenerCreditosPorClienteCasoUso _obtenerCreditosPorClienteCasoUso;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IZonaCobranzaRepositorio _zonaCobranzaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public ClientesController(
            ObtenerClientesCasoUso obtenerCasoUso,
            CrearClienteCasoUso crearCasoUso,
            ActualizarClienteCasoUso actualizarCasoUso,
            EliminarClienteCasoUso eliminarCasoUso,
            ObtenerCreditosPorClienteCasoUso obtenerCreditosPorClienteCasoUso,
            IUsuarioRepositorio usuarioRepositorio,
            IZonaCobranzaRepositorio zonaCobranzaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _obtenerCasoUso = obtenerCasoUso;
            _crearCasoUso = crearCasoUso;
            _actualizarCasoUso = actualizarCasoUso;
            _eliminarCasoUso = eliminarCasoUso;
            _obtenerCreditosPorClienteCasoUso = obtenerCreditosPorClienteCasoUso;
            _usuarioRepositorio = usuarioRepositorio;
            _zonaCobranzaRepositorio = zonaCobranzaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<ClientesListadoDto>>> ObtenerTodos(
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] string? buscar,
            [FromQuery] Guid? zonaId)
        {
            var userId = CurrentUser.GetUserId(User);
            var zonaRes = await CobranzaZonaFiltroResolver.ResolverAsync(
                userId,
                zonaId,
                _usuarioRepositorio,
                _zonaCobranzaRepositorio,
                _permisoBotonRepositorio,
                CobranzaZonaFiltroResolver.PermisoClienteTodasZonas);

            if (zonaRes.TieneError)
            {
                return Ok(ApiResponse<ClientesListadoDto>.Fail(zonaRes.MensajeError ?? string.Empty, zonaRes.CodigoError));
            }

            var listado = await _obtenerCasoUso.Ejecutar(page, pageSize, buscar, zonaRes.ZonaId, zonaRes.AplicarFiltroZona);
            return Ok(ApiResponse<ClientesListadoDto>.Success(listado));
        }

        [HttpPost]
        [RequireBotonPermiso("CLIENTE_CREAR")]
        public async Task<ActionResult<ApiResponse<ClienteDto>>> Crear([FromBody] ClienteDto dto)
        {
            try
            {
                var userId = CurrentUser.GetUserId(User);
                var created = await _crearCasoUso.Ejecutar(dto, userId);
                return Ok(ApiResponse<ClienteDto>.Success(created, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<ClienteDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPut("{id:guid}")]
        [RequireBotonPermisoAlguno("CLIENTE_CREAR", "CLIENTE_EDITAR")]
        public async Task<ActionResult<ApiResponse<ClienteDto>>> Actualizar(Guid id, [FromBody] ClienteDto dto)
        {
            try
            {
                var userId = CurrentUser.GetUserId(User);
                var updated = await _actualizarCasoUso.Ejecutar(id, dto, userId);
                return Ok(ApiResponse<ClienteDto>.Success(updated));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<ClienteDto>.Fail(ex.Message, 400));
            }
        }

        [HttpDelete("{id:guid}")]
        [RequireBotonPermisoAlguno("CLIENTE_CREAR", "CLIENTE_ELIMINAR")]
        public async Task<ActionResult<ApiResponse<bool>>> Eliminar(Guid id)
        {
            try
            {
                var userId = CurrentUser.GetUserId(User);
                await _eliminarCasoUso.Ejecutar(id, userId);
                return Ok(ApiResponse<bool>.Success(true));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<bool>.Fail(ex.Message, 400));
            }
        }

        [HttpGet("{id:guid}/creditos")]
        public async Task<ActionResult<ApiResponse<ClienteCreditosDto>>> ObtenerCreditos(Guid id)
        {
            try
            {
                var result = await _obtenerCreditosPorClienteCasoUso.Ejecutar(id);
                return Ok(ApiResponse<ClienteCreditosDto>.Success(result));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<ClienteCreditosDto>.Fail(ex.Message, 400));
            }
        }
    }
}
