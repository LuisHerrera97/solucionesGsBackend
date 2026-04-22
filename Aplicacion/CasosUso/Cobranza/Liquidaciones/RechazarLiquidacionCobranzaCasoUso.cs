using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Enums.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Cobranza.Liquidaciones
{
    public class RechazarLiquidacionCobranzaCasoUso
    {
        private readonly ILiquidacionCobranzaRepositorio _liquidacionRepositorio;
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public RechazarLiquidacionCobranzaCasoUso(
            ILiquidacionCobranzaRepositorio liquidacionRepositorio,
            IMovimientoCajaRepositorio movimientoRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IClock clock,
            IUnitOfWork unitOfWork)
        {
            _liquidacionRepositorio = liquidacionRepositorio;
            _movimientoRepositorio = movimientoRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task Ejecutar(Guid liquidacionId, Guid adminId)
        {
            var liquidacion = await _liquidacionRepositorio.GetByIdAsync(liquidacionId);
            if (liquidacion == null) throw new ArgumentException("Liquidación no encontrada");
            if (!EstatusLiquidacionCobranzaExtensions.EqualsStored(liquidacion.Estatus, EstatusLiquidacionCobranza.Enviada))
                throw new ArgumentException("La liquidación ya fue procesada o no está en estado Enviada");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Actualizar estatus de la liquidación
                liquidacion.Estatus = EstatusLiquidacionCobranza.Rechazada.ToStoredString();
                liquidacion.ConfirmadaPorId = adminId;
                liquidacion.FechaConfirmacion = DateTime.Now;
                await _liquidacionRepositorio.UpdateAsync(liquidacion);

                // 2. Desvincular movimientos para que aparezcan como pendientes de liquidar nuevamente
                await _movimientoRepositorio.DesvincularLiquidacionAsync(liquidacion.Id);

                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = adminId,
                    Accion = "RechazarLiquidacionCobranza",
                    EntidadTipo = "LiquidacionCobranza",
                    EntidadId = liquidacion.Id,
                    Fecha = _clock.UtcNow,
                    Detalle = $"Total:{liquidacion.Total};CobradorId:{liquidacion.CobradorId}"
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
