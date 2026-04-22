using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;

namespace FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja
{
    public interface IMovimientoCajaRepositorio
    {
        Task<IEnumerable<MovimientoCaja>> ObtenerTurnoAsync(DateTime? fechaDia = null);
        Task<IEnumerable<MovimientoCaja>> ObtenerEnRangoAsync(
            DateTime fechaDesde,
            DateTime fechaHasta,
            int? page = null,
            int? pageSize = null,
            Guid? cobradorId = null,
            Guid? zonaId = null);
        Task<IEnumerable<MovimientoCaja>> ObtenerPendientesLiquidacionAsync(Guid cobradorId, DateTime fecha);
        Task<MovimientoCaja> AddAsync(MovimientoCaja movimiento);
        Task<MovimientoCaja> GetByIdAsync(Guid id);
        Task<MovimientoCaja?> GetByIdempotencyKeyAsync(string idempotencyKey);
        Task<bool> TienePendientesCorteAsync(DateTime fecha);
        Task<bool> TienePendientesLiquidacionAsync(DateTime fecha);
        Task<int> AsignarCorteAsync(Guid corteId, DateTime fechaCorte);
        Task<int> AsignarLiquidacionAsync(Guid liquidacionId, Guid cobradorId, DateTime fecha);
        Task<int> DesvincularLiquidacionAsync(Guid liquidacionId);
        Task<IEnumerable<MovimientoCaja>> ObtenerPorCreditoAsync(Guid creditoId);
        Task<int> MarcarRecibidoCajaAsync(IEnumerable<Guid> movimientoIds, DateTime fechaDia);
    }
}
