using System;
using FinancieraSoluciones.Api.Utils;
using FinancieraSoluciones.Application.CasosUso.Seguridad;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Application.DTOs.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancieraSoluciones.Api.Controllers.Seguridad
{
    [ApiController]
    [Route("api/autenticacion")]
    public class AuthController : ControllerBase
    {
        private readonly AutenticarUsuarioCasoUso _autenticarUsuarioCasoUso;
        private readonly RefrescarTokenCasoUso _refrescarTokenCasoUso;
        private readonly SolicitarRecuperacionPasswordCasoUso _solicitarRecuperacionPasswordCasoUso;
        private readonly RestablecerPasswordCasoUso _restablecerPasswordCasoUso;
        private readonly CambiarPasswordCasoUso _cambiarPasswordCasoUso;

        public AuthController(
            AutenticarUsuarioCasoUso autenticarUsuarioCasoUso,
            RefrescarTokenCasoUso refrescarTokenCasoUso,
            SolicitarRecuperacionPasswordCasoUso solicitarRecuperacionPasswordCasoUso,
            RestablecerPasswordCasoUso restablecerPasswordCasoUso,
            CambiarPasswordCasoUso cambiarPasswordCasoUso)
        {
            _autenticarUsuarioCasoUso = autenticarUsuarioCasoUso;
            _refrescarTokenCasoUso = refrescarTokenCasoUso;
            _solicitarRecuperacionPasswordCasoUso = solicitarRecuperacionPasswordCasoUso;
            _restablecerPasswordCasoUso = restablecerPasswordCasoUso;
            _cambiarPasswordCasoUso = cambiarPasswordCasoUso;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AutenticacionResponseDto>>> Login([FromBody] AutenticacionRequestDto request)
        {
            try
            {
                var resultado = await _autenticarUsuarioCasoUso.Ejecutar(request);

                if (!resultado.Autenticado)
                {
                    return Ok(ApiResponse<AutenticacionResponseDto>.Fail("Credenciales inválidas", 401));
                }

                return Ok(ApiResponse<AutenticacionResponseDto>.Success(resultado));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<AutenticacionResponseDto>.Fail(ex.Message, 403));
            }
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AutenticacionResponseDto>>> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var resultado = await _refrescarTokenCasoUso.Ejecutar(request);
                return Ok(ApiResponse<AutenticacionResponseDto>.Success(resultado));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<AutenticacionResponseDto>.Fail(ex.Message, 401));
            }
        }

        [HttpPost("recuperar-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<SolicitarRecuperacionPasswordResponseDto>>> SolicitarRecuperacion([FromBody] SolicitarRecuperacionPasswordRequestDto request)
        {
            try
            {
                var result = await _solicitarRecuperacionPasswordCasoUso.Ejecutar(request);
                return Ok(ApiResponse<SolicitarRecuperacionPasswordResponseDto>.Success(result));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<SolicitarRecuperacionPasswordResponseDto>.Fail(ex.Message, 400));
            }
        }

        [HttpPost("restablecer-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<object>>> RestablecerPassword([FromBody] RestablecerPasswordRequestDto request)
        {
            try
            {
                await _restablecerPasswordCasoUso.Ejecutar(request);
                return Ok(ApiResponse<object>.Success(new { ok = true }, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<object>.Fail(ex.Message, 400));
            }
        }

        [HttpPost("cambiar-password")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> CambiarPassword([FromBody] CambiarPasswordRequestDto request)
        {
            var userId = CurrentUser.GetUserId(User);
            if (!userId.HasValue)
            {
                return Ok(ApiResponse<object>.Fail("Usuario no autenticado", 401));
            }

            try
            {
                await _cambiarPasswordCasoUso.Ejecutar(userId.Value, request);
                return Ok(ApiResponse<object>.Success(new { ok = true }, httpCode: 201));
            }
            catch (ArgumentException ex)
            {
                return Ok(ApiResponse<object>.Fail(ex.Message, 400));
            }
        }
    }
}
