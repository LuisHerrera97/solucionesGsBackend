using System;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Entidades.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class CondonarInteresMontoCasoUso
    {
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IMovimientoCajaRepositorio _movimientoCajaRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public CondonarInteresMontoCasoUso(
            ICreditoRepositorio creditoRepositorio,
            IMovimientoCajaRepositorio movimientoCajaRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IClock clock,
            IUnitOfWork unitOfWork)
        {
            _creditoRepositorio = creditoRepositorio;
            _movimientoCajaRepositorio = movimientoCajaRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task Ejecutar(Guid creditoId, decimal montoACondonar, Guid? usuarioId)
        {
            if (montoACondonar <= 0) throw new ArgumentException("El monto a condonar debe ser mayor a 0");

            var credito = await _creditoRepositorio.GetByIdAsync(creditoId);
            if (credito == null) throw new ArgumentException("Crédito no encontrado");

            var fichasPendientes = credito.Fichas
                .Where(f => !f.Pagada)
                .OrderBy(f => f.Num)
                .ToList();

            var interesTotalPendiente = fichasPendientes.Sum(f => f.Interes);

            if (interesTotalPendiente <= 0)
                throw new InvalidOperationException("No hay interés pendiente por condonar en las fichas no pagadas.");

            if (montoACondonar > interesTotalPendiente)
            {
                throw new ArgumentException("El monto a condonar no puede ser mayor al total de intereses no pagados");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                decimal saldoRestante = montoACondonar;
                decimal totalCondonadoReal = 0;

                foreach (var ficha in fichasPendientes)
                {
                    if (saldoRestante <= 0) break;

                    decimal interesFicha = ficha.Interes;
                    if (interesFicha <= 0) continue;

                    decimal descontar = Math.Min(saldoRestante, interesFicha);

                    ficha.Interes -= descontar;

                    ficha.Total = (ficha.Capital + ficha.Interes + ficha.MoraAcumulada) - ficha.AbonoAcumulado;
                    ficha.SaldoPendiente = ficha.Total;
                    if (ficha.Total < 0) { ficha.Total = 0; ficha.SaldoPendiente = 0; }

                    saldoRestante -= descontar;
                    totalCondonadoReal += descontar;

                    if (descontar > 0)
                    {
                        var movimiento = new MovimientoCaja
                        {
                            Id = Guid.NewGuid(),
                            Tipo = TipoMovimientoCaja.CondonacionInteres.ToStoredString(),
                            Concepto = $"Condonación de interés (Ficha #{ficha.Num})",
                            Medio = MedioMovimientoCaja.Ajuste.ToStoredString(),
                            Total = descontar,
                            CreditoId = credito.Id,
                            NumeroFicha = ficha.Num,
                            Fecha = _clock.Today,
                            Hora = _clock.Now.ToString("HH:mm"),
                            CobradorId = usuarioId,
                            RegistraCaja = false
                        };
                        await _movimientoCajaRepositorio.AddAsync(movimiento);
                    }
                }

                credito.InteresTotal -= totalCondonadoReal;
                credito.Total -= totalCondonadoReal;

                await _creditoRepositorio.UpdateAsync(credito);

                if (totalCondonadoReal > 0)
                {
                    await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                    {
                        Id = Guid.NewGuid(),
                        UsuarioId = usuarioId,
                        Accion = "CondonarInteresMonto",
                        EntidadTipo = "Credito",
                        EntidadId = credito.Id,
                        Fecha = _clock.UtcNow,
                        Detalle = $"MontoSolicitado:{montoACondonar};MontoCondonado:{totalCondonadoReal}"
                    });
                }

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
