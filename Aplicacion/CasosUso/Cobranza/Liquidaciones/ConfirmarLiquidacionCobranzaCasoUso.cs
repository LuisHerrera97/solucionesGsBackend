using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Enums.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Cobranza.Liquidaciones
{
    public class ConfirmarLiquidacionCobranzaCasoUso
    {
        private readonly ILiquidacionCobranzaRepositorio _liquidacionRepositorio;
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmarLiquidacionCobranzaCasoUso(
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
                liquidacion.Estatus = EstatusLiquidacionCobranza.Confirmada.ToStoredString();
                liquidacion.ConfirmadaPorId = adminId;
                liquidacion.FechaConfirmacion = DateTime.Now;
                await _liquidacionRepositorio.UpdateAsync(liquidacion);

                // 2. Crear movimiento en Caja General (Entrada por Cobranza)
                var movimiento = new MovimientoCaja
                {
                    Id = Guid.NewGuid(),
                    LiquidacionCobranzaId = liquidacion.Id,
                    CobradorId = liquidacion.CobradorId,
                    Tipo = TipoMovimientoCaja.Entrada.ToStoredString(),
                    Concepto = $"Liquidación de cobranza confirmada (ID: {liquidacion.Id.ToString().Substring(0, 8)})",
                    Medio = MedioMovimientoCaja.Mixto.ToStoredString(),
                    Total = liquidacion.Total,
                    MontoEfectivo = liquidacion.TotalEfectivo,
                    MontoTransferencia = liquidacion.TotalTransferencia,
                    Fecha = DateTime.Today,
                    Hora = DateTime.Now.ToString("HH:mm"),
                    RegistraCaja = true
                };
                await _movimientoRepositorio.AddAsync(movimiento);

                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = adminId,
                    Accion = "ConfirmarLiquidacionCobranza",
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
