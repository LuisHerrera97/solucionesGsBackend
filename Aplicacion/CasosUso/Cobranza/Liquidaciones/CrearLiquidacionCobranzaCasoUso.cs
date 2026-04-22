using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Entidades.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Enums.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Cobranza.Liquidaciones
{
    public class CrearLiquidacionCobranzaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly ILiquidacionCobranzaRepositorio _liquidacionRepositorio;
        private readonly IMovimientoCajaRepositorio _movimientoCajaRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public CrearLiquidacionCobranzaCasoUso(
            IMapper mapper,
            ILiquidacionCobranzaRepositorio liquidacionRepositorio,
            IMovimientoCajaRepositorio movimientoCajaRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _liquidacionRepositorio = liquidacionRepositorio;
            _movimientoCajaRepositorio = movimientoCajaRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<LiquidacionCobranzaDto> Ejecutar(Guid cobradorId, CrearLiquidacionCobranzaRequestDto request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {

                var movimientos = (await _movimientoCajaRepositorio.ObtenerPendientesLiquidacionAsync(cobradorId, DateTime.Today)).ToList();
                if (movimientos.Count == 0) throw new ArgumentException("No hay movimientos pendientes de liquidación");

                var totalEfectivo = movimientos.Sum(m => m.MontoEfectivo ?? 0);
                var totalTransferencia = movimientos.Sum(m => m.MontoTransferencia ?? 0);
                var total = movimientos.Sum(m => m.Total);

                var liquidacion = new LiquidacionCobranza
                {
                    Id = Guid.NewGuid(),
                    CobradorId = cobradorId,
                    Fecha = DateTime.Today,
                    Hora = DateTime.Now.ToString("HH:mm"),
                    TotalEfectivo = totalEfectivo,
                    TotalTransferencia = totalTransferencia,
                    Total = total,
                    Evidencia = request?.Evidencia?.Trim(),
                    Estatus = EstatusLiquidacionCobranza.Enviada.ToStoredString(),
                    FechaCreacion = DateTime.UtcNow,
                    ConfirmadaPorId = null,
                    FechaConfirmacion = null
                };

                await _liquidacionRepositorio.AddAsync(liquidacion);
                await _movimientoCajaRepositorio.AsignarLiquidacionAsync(liquidacion.Id, cobradorId, DateTime.Today);

                await _auditoriaRepositorio.AddAsync(new Domain.Entidades.General.AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = cobradorId,
                    Accion = "CrearLiquidacionCobranza",
                    EntidadTipo = "LiquidacionCobranza",
                    EntidadId = liquidacion.Id,
                    Fecha = DateTime.UtcNow,
                    Detalle = $"Movimientos:{movimientos.Count};Total:{total}"
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return _mapper.Map<LiquidacionCobranzaDto>(liquidacion);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
