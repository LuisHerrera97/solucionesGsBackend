using System;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Finanzas.Caja;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas.Caja
{
    public class MarcarMovimientosRecibidoCajaCasoUso
    {
        private readonly IMovimientoCajaRepositorio _movimientoRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public MarcarMovimientosRecibidoCajaCasoUso(
            IMovimientoCajaRepositorio movimientoRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _movimientoRepositorio = movimientoRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Ejecutar(Guid usuarioId, MarcarRecibidoCajaRequestDto request)
        {
            if (request == null || request.MovimientoIds == null || request.MovimientoIds.Count == 0)
            {
                throw new ArgumentException("Debe indicar al menos un movimiento");
            }

            var fecha = (request.Fecha ?? DateTime.Today).Date;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var count = await _movimientoRepositorio.MarcarRecibidoCajaAsync(request.MovimientoIds, fecha);
                if (count == 0)
                {
                    throw new ArgumentException("No se pudo marcar ningún movimiento (verifica fecha, tipo y que no tengan liquidación ni corte)");
                }

                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Accion = "MarcarMovimientosRecibidoCaja",
                    EntidadTipo = "MovimientoCaja",
                    EntidadId = request.MovimientoIds.First(),
                    Fecha = DateTime.UtcNow,
                    Detalle = $"Cantidad:{count};Fecha:{fecha:yyyy-MM-dd}",
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return count;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
