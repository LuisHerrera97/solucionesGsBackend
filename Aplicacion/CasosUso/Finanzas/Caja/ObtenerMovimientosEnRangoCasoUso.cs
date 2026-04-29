using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Application.Finanzas.Caja;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Caja
{
    public class ObtenerMovimientosEnRangoCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;

        public ObtenerMovimientosEnRangoCasoUso(
            IMapper mapper,
            IMovimientoCajaRepositorio movimientoRepositorio)
        {
            _mapper = mapper;
            _movimientoRepositorio = movimientoRepositorio;
        }

        public async Task<IEnumerable<MovimientoCajaDto>> Ejecutar(
            DateTime fechaDesde,
            DateTime fechaHasta,
            int? page = null,
            int? pageSize = null,
            Guid? cobradorId = null,
            Guid? zonaId = null,
            string? creditoFolio = null,
            string? clienteNombre = null)
        {
            var movimientos = (await _movimientoRepositorio.ObtenerEnRangoAsync(
                fechaDesde,
                fechaHasta,
                page,
                pageSize,
                cobradorId,
                zonaId,
                creditoFolio,
                clienteNombre)).ToList();

            return movimientos.Select(m =>
            {
                var dto = _mapper.Map<MovimientoCajaDto>(m);
                dto.EstatusFichaFinanzas = MovimientoCajaEstatusFinanzasHelper.CalcularEstatusFichaFinanzas(m, null);
                return dto;
            });
        }

        public async Task<IEnumerable<MovimientoCajaCobranzaDto>> EjecutarParaCobranza(
            DateTime fechaDesde,
            DateTime fechaHasta,
            int? page = null,
            int? pageSize = null,
            Guid? cobradorId = null,
            Guid? zonaId = null,
            string? creditoFolio = null,
            string? clienteNombre = null)
        {
            var movimientos = (await _movimientoRepositorio.ObtenerEnRangoAsync(
                fechaDesde,
                fechaHasta,
                page,
                pageSize,
                cobradorId,
                zonaId,
                creditoFolio,
                clienteNombre)).ToList();

            var revertidosIds = movimientos
                .Where(m => m.ReversaDeId.HasValue)
                .Select(m => m.ReversaDeId!.Value)
                .ToHashSet();

            return movimientos.Select(m => MapCobranza(m, revertidosIds.Contains(m.Id)));
        }

        private static MovimientoCajaCobranzaDto MapCobranza(MovimientoCaja m, bool revertido)
        {
            var nombreCliente = m.Credito?.Cliente != null
                ? string.Join(' ', new[] { m.Credito.Cliente.Nombre, m.Credito.Cliente.Apellido }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim()
                : null;

            return new MovimientoCajaCobranzaDto
            {
                Id = m.Id,
                Tipo = m.Tipo ?? string.Empty,
                Concepto = m.Concepto ?? string.Empty,
                Total = m.Total,
                Abono = m.Abono,
                Mora = m.Mora,
                OperacionId = m.OperacionId,
                ReversaDeId = m.ReversaDeId,
                Revertido = revertido,
                CreditoId = m.CreditoId,
                CreditoFolio = m.Credito?.Folio,
                ClienteNombre = string.IsNullOrWhiteSpace(nombreCliente) ? null : nombreCliente,
                NumeroFicha = m.NumeroFicha,
                Fecha = m.Fecha,
                Hora = m.Hora ?? string.Empty,
            };
        }
    }
}
