using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Application.Servicios.Finanzas;
using FinancieraSoluciones.Domain.Common;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Domain.Entidades.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class PenalizarFichaManualCasoUso
    {
        private readonly IMapper _mapper;
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IMovimientoCajaRepositorio _movimientoCajaRepositorio;
        private readonly ICreditoZonaAutorizacionService _creditoZonaAutorizacionService;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public PenalizarFichaManualCasoUso(
            IMapper mapper,
            ICreditoRepositorio creditoRepositorio,
            IMovimientoCajaRepositorio movimientoCajaRepositorio,
            ICreditoZonaAutorizacionService creditoZonaAutorizacionService,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IClock clock,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _creditoRepositorio = creditoRepositorio;
            _movimientoCajaRepositorio = movimientoCajaRepositorio;
            _creditoZonaAutorizacionService = creditoZonaAutorizacionService;
            _auditoriaRepositorio = auditoriaRepositorio;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreditoDto> Ejecutar(Guid creditoId, int numeroFicha, PenalizarFichaRequestDto request, Guid? usuarioId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var credito = await _creditoRepositorio.GetByIdAsync(creditoId);
                if (credito == null) throw new NotFoundException("No existe el crédito");

                await _creditoZonaAutorizacionService.AsegurarPuedeOperarAsync(credito, usuarioId);

                var ficha = credito.Fichas?.FirstOrDefault(f => f.Num == numeroFicha);
                if (ficha == null) throw new NotFoundException("No existe la ficha");
                if (ficha.Pagada) throw new BusinessRuleException("La ficha ya está pagada");

                var monto = request.Monto;
                if (monto <= 0) throw new BusinessRuleException("El monto debe ser mayor a 0");

                ficha.MoraAcumulada += monto;

                ficha.Total = (ficha.Capital + ficha.Interes + ficha.MoraAcumulada) - ficha.AbonoAcumulado;
                ficha.SaldoPendiente = ficha.Total;
                if (ficha.Total < 0) { ficha.Total = 0; ficha.SaldoPendiente = 0; }

                await _creditoRepositorio.UpdateAsync(credito);

                var movimiento = new MovimientoCaja
                {
                    Id = Guid.NewGuid(),
                    IdempotencyKey = request.IdempotencyKey,
                    Tipo = TipoMovimientoCaja.Penalizacion.ToStoredString(),
                    Concepto = $"Cargo por Penalización Manual (Ficha #{numeroFicha})",
                    Medio = MedioMovimientoCaja.Ajuste.ToStoredString(),
                    Total = 0,
                    Abono = 0,
                    Mora = monto,
                    CreditoId = credito.Id,
                    NumeroFicha = numeroFicha,
                    Fecha = _clock.Today,
                    Hora = _clock.Now.ToString("HH:mm"),
                    CobradorId = usuarioId,
                    RegistraCaja = true
                };

                await _movimientoCajaRepositorio.AddAsync(movimiento);

                await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Accion = "MultaFicha",
                    EntidadTipo = "Credito",
                    EntidadId = credito.Id,
                    Fecha = _clock.UtcNow,
                    Detalle = $"Ficha:{numeroFicha};Mora:{monto};Concepto:{movimiento.Concepto}"
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var dto = _mapper.Map<CreditoDto>(credito);
                dto.Fichas = (credito.Fichas ?? Enumerable.Empty<Domain.Entidades.Finanzas.Ficha>())
                    .OrderBy(f => f.Num)
                    .Select(f => _mapper.Map<FichaDto>(f))
                    .ToList();
                return dto;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
