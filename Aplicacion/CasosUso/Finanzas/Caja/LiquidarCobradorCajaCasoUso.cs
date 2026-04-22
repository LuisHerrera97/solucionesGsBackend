using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.CasosUso.Cobranza.Liquidaciones;
using FinancieraSoluciones.Application.DTOs.Cobranza.Liquidaciones;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Domain.Common;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Caja
{
    public class LiquidarCobradorCajaCasoUso
    {
        private const string PermisoTodasZonas = "CREDITO_TODAS_ZONAS";

        private readonly CrearLiquidacionCobranzaCasoUso _crearLiquidacionCasoUso;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public LiquidarCobradorCajaCasoUso(
            CrearLiquidacionCobranzaCasoUso crearLiquidacionCasoUso,
            IUsuarioRepositorio usuarioRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _crearLiquidacionCasoUso = crearLiquidacionCasoUso;
            _usuarioRepositorio = usuarioRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        public async Task<LiquidacionCobranzaDto> Ejecutar(Guid usuarioId, LiquidarCobradorRequestDto request)
        {
            if (request == null) throw new ArgumentException("Request requerido");
            if (request.CobradorId == Guid.Empty) throw new ArgumentException("Cobrador inválido");

            var objetivoId = request.CobradorId;
            if (objetivoId != usuarioId)
            {
                var usuario = await _usuarioRepositorio.GetByIdAsync(usuarioId);
                var tienePermiso = usuario != null && await _permisoBotonRepositorio.HasPermisoAsync(usuario.IdPerfil, PermisoTodasZonas);
                if (!tienePermiso)
                {
                    throw new ForbiddenException("No tienes permiso para liquidar cobradores de otras zonas");
                }
            }

            return await _crearLiquidacionCasoUso.Ejecutar(
                objetivoId,
                new CrearLiquidacionCobranzaRequestDto
                {
                    Evidencia = request.Evidencia,
                });
        }
    }
}
