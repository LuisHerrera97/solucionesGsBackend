using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.DTOs.Finanzas.Cortes;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Cortes;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Cortes
{
    public class ObtenerCortesCasoUso
    {
        private readonly IMapper _mapper;
        private readonly ICorteCajaRepositorio _corteRepositorio;

        public ObtenerCortesCasoUso(IMapper mapper, ICorteCajaRepositorio corteRepositorio)
        {
            _mapper = mapper;
            _corteRepositorio = corteRepositorio;
        }

        public async Task<IEnumerable<CorteCajaDto>> Ejecutar(DateTime fechaInicio, DateTime fechaFin, int? page = null, int? pageSize = null)
        {
            var cortes = await _corteRepositorio.ObtenerEnRangoAsync(fechaInicio, fechaFin, page, pageSize);
            return cortes.Select(c =>
            {
                var dto = _mapper.Map<CorteCajaDto>(c);
                dto.Movimientos = (c.Movimientos ?? new List<Domain.Entidades.Finanzas.Caja.MovimientoCaja>())
                    .OrderBy(m => m.Fecha)
                    .ThenBy(m => m.Hora)
                    .Select(m => _mapper.Map<MovimientoCajaDto>(m))
                    .ToList();
                return dto;
            });
        }
    }
}
