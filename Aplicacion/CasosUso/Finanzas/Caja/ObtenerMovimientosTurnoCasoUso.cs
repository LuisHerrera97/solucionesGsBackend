using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Caja
{
    public class ObtenerMovimientosTurnoCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;

        public ObtenerMovimientosTurnoCasoUso(IMapper mapper, IMovimientoCajaRepositorio movimientoRepositorio)
        {
            _mapper = mapper;
            _movimientoRepositorio = movimientoRepositorio;
        }

        public async Task<IEnumerable<MovimientoCajaDto>> Ejecutar(DateTime? fecha = null)
        {
            var movimientos = await _movimientoRepositorio.ObtenerTurnoAsync(fecha);
            return movimientos.Select(m => _mapper.Map<MovimientoCajaDto>(m));
        }
    }
}
