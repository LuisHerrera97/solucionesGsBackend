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
    public class CondonarInteresFichaCasoUso
    {
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IMovimientoCajaRepositorio _movimientoCajaRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public CondonarInteresFichaCasoUso(
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

        public async Task Ejecutar(Guid creditoId, int numeroFicha, Guid? usuarioId)
        {
            var credito = await _creditoRepositorio.GetByIdAsync(creditoId);
            if (credito == null) throw new ArgumentException("No existe el crédito");

            var ficha = credito.Fichas.FirstOrDefault(f => f.Num == numeroFicha);
            if (ficha == null) throw new ArgumentException("No existe la ficha");
            if (ficha.Pagada) throw new InvalidOperationException("No se puede condonar interés de una ficha pagada");

            var interesCondonado = ficha.Interes;
            if (interesCondonado <= 0)
                throw new InvalidOperationException("No hay interés por condonar en esta ficha.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                ficha.Interes = 0;

                ficha.Total = (ficha.Capital + ficha.Interes + ficha.MoraAcumulada) - ficha.AbonoAcumulado;
                ficha.SaldoPendiente = ficha.Total;
                if (ficha.Total < 0) { ficha.Total = 0; ficha.SaldoPendiente = 0; }

                credito.InteresTotal = Math.Max(0, credito.InteresTotal - interesCondonado);
                credito.Total = Math.Max(0, credito.Total - interesCondonado);

                await _creditoRepositorio.UpdateAsync(credito);

                var movimiento = new MovimientoCaja
                {
                    Id = Guid.NewGuid(),
                    Tipo = TipoMovimientoCaja.CondonacionInteres.ToStoredString(),
                    Concepto = $"Condonación de interés (Ficha #{numeroFicha})",
                    Medio = MedioMovimientoCaja.Ajuste.ToStoredString(),
                    Total = interesCondonado,
                    CreditoId = credito.Id,
                    NumeroFicha = numeroFicha,
                    Fecha = _clock.Today,
                    Hora = _clock.Now.ToString("HH:mm"),
                    CobradorId = usuarioId,
                    RegistraCaja = false
                };
                await _movimientoCajaRepositorio.AddAsync(movimiento);

                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Accion = "CondonarInteresFicha",
                    EntidadTipo = "Credito",
                    EntidadId = credito.Id,
                    Fecha = _clock.UtcNow,
                    Detalle = $"Ficha:{numeroFicha};InteresCondonado:{interesCondonado}"
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
