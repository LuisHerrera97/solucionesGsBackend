using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Caja
{
    public class ObtenerMovimientosPorCreditoCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;

        public ObtenerMovimientosPorCreditoCasoUso(IMapper mapper, IMovimientoCajaRepositorio movimientoRepositorio)
        {
            _mapper = mapper;
            _movimientoRepositorio = movimientoRepositorio;
        }

        public async Task<IEnumerable<MovimientoCajaDto>> Ejecutar(Guid creditoId)
        {
            var movimientos = (await _movimientoRepositorio.ObtenerPorCreditoAsync(creditoId)).ToList();
            var revertidosIds = movimientos
                .Where(m => m.ReversaDeId.HasValue)
                .Select(m => m.ReversaDeId!.Value)
                .ToHashSet();

            return movimientos.Select(m =>
            {
                var dto = _mapper.Map<MovimientoCajaDto>(m);
                dto.Revertido = revertidosIds.Contains(m.Id);
                return dto;
            });
        }
    }
}
