using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Cobranza.Liquidaciones
{
    public class ObtenerTodasLiquidacionesCasoUso
    {
        private readonly ILiquidacionCobranzaRepositorio _liquidacionRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;

        public ObtenerTodasLiquidacionesCasoUso(
            ILiquidacionCobranzaRepositorio liquidacionRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IMapper mapper)
        {
            _liquidacionRepositorio = liquidacionRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LiquidacionCobranzaDto>> Ejecutar(DateTime fechaInicio, DateTime fechaFin, Guid? zonaId = null)
        {
            var liquidaciones = await _liquidacionRepositorio.GetTodasAsync(fechaInicio, fechaFin, zonaId);
            var dtos = _mapper.Map<List<LiquidacionCobranzaDto>>(liquidaciones);

            if (dtos.Count > 0)
            {
                var cobradorIds = dtos.Select(d => d.CobradorId).Distinct().ToList();
                var nombresCobradores = new Dictionary<Guid, string>();

                foreach (var id in cobradorIds)
                {
                    var u = await _usuarioRepositorio.GetByIdAsync(id);
                    if (u != null)
                    {
                        nombresCobradores[id] = $"{u.Nombre} {u.ApellidoPaterno}".Trim();
                    }
                }

                foreach (var dto in dtos)
                {
                    if (nombresCobradores.TryGetValue(dto.CobradorId, out var nombre))
                    {
                        dto.NombreCobrador = nombre;
                    }
                }
            }

            return dtos;
        }
    }
}
