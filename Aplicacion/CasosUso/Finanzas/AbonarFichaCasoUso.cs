using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Application.Servicios.Finanzas;
using FinancieraSoluciones.Domain.Common;
using FinancieraSoluciones.Domain.Entidades.Finanzas;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Enums.Finanzas;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class AbonarFichaCasoUso
    {
        private readonly IMapper _mapper;
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IMovimientoCajaRepositorio _movimientoCajaRepositorio;
        private readonly ICreditoZonaAutorizacionService _creditoZonaAutorizacionService;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public AbonarFichaCasoUso(
            IMapper mapper,
            ICreditoRepositorio creditoRepositorio,
            IConfiguracionSistemaRepositorio configuracionRepositorio,
            IMovimientoCajaRepositorio movimientoCajaRepositorio,
            ICreditoZonaAutorizacionService creditoZonaAutorizacionService,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IClock clock,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _creditoRepositorio = creditoRepositorio;
            _configuracionRepositorio = configuracionRepositorio;
            _movimientoCajaRepositorio = movimientoCajaRepositorio;
            _creditoZonaAutorizacionService = creditoZonaAutorizacionService;
            _auditoriaRepositorio = auditoriaRepositorio;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreditoDto> Ejecutar(Guid creditoId, int numeroFicha, AbonarFichaRequestDto request, Guid? usuarioId)
        {
            var operacionId = Guid.NewGuid();
            var operacionKey = Guid.NewGuid().ToString("N");
            var idempotencyKey = request?.IdempotencyKey?.Trim();
            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                var existente = await _movimientoCajaRepositorio.GetByIdempotencyKeyAsync(idempotencyKey);
                if (existente != null &&
                    existente.CreditoId == creditoId &&
                    existente.NumeroFicha == numeroFicha &&
                    TipoMovimientoCajaExtensions.EqualsStored(existente.Tipo, TipoMovimientoCaja.Ficha))
                {
                    var creditoPrevio = await _creditoRepositorio.GetByIdAsync(creditoId);
                    if (creditoPrevio == null) throw new NotFoundException("No existe el crédito");
                    return MapCreditoConFichasOrdenadas(creditoPrevio);
                }
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {

                var credito = await _creditoRepositorio.GetByIdAsync(creditoId);
                if (credito == null) throw new NotFoundException("No existe el crédito");

                await _creditoZonaAutorizacionService.AsegurarPuedeOperarAsync(credito, usuarioId);

                var ficha = credito.Fichas?.FirstOrDefault(f => f.Num == numeroFicha);
                if (ficha == null) throw new NotFoundException("No existe la ficha");
                if (ficha.Pagada) throw new BusinessRuleException("La ficha ya está pagada");

                var config = await _configuracionRepositorio.GetAsync();
                if (config == null) throw new NotFoundException("No existe configuración del sistema");

                var adeudoFicha = ficha.Capital + ficha.Interes + ficha.MoraAcumulada;
                var faltanteTotal = Math.Max(0, adeudoFicha - ficha.AbonoAcumulado);

                var montoTotalRecibido = request?.MontoAbono ?? faltanteTotal;
                if (montoTotalRecibido < 0) throw new BusinessRuleException("El monto no puede ser negativo");
                if (montoTotalRecibido == 0) throw new BusinessRuleException("El monto debe ser mayor a 0");

                var abonoADepositar = montoTotalRecibido;

                if (abonoADepositar - (faltanteTotal + DecimalTolerance.Centavo) > 0)
                    throw new BusinessRuleException("El pago excede el total pendiente de la ficha");

                var medioPago = ParseMedioPagoCobro(request?.Medio);

                var esPagoCompleto = Math.Abs(abonoADepositar - faltanteTotal) <= DecimalTolerance.Centavo;
                if (!esPagoCompleto)
                {
                    ficha.AbonoAcumulado += abonoADepositar;
                    ficha.Total = (ficha.Capital + ficha.Interes + ficha.MoraAcumulada) - ficha.AbonoAcumulado;
                    ficha.SaldoPendiente = ficha.Total;
                    if (ficha.Total < 0) { ficha.Total = 0; ficha.SaldoPendiente = 0; }
                }

                if (esPagoCompleto || ficha.AbonoAcumulado >= (adeudoFicha - DecimalTolerance.Centavo))
                {
                    ficha.Pagada = true;
                    ficha.FechaPago = _clock.Today;
                    ficha.Hora = _clock.Now.ToString("HH:mm");
                    ficha.Cerrada = true;
                    ficha.FechaCierre = _clock.UtcNow;
                    ficha.Total = 0;
                    ficha.SaldoPendiente = 0;
                }

                credito.Pagado += abonoADepositar;
                if (credito.Fichas.All(f => f.Pagada))
                {
                    credito.Estatus = EstatusCredito.Liquidado.ToStoredString();
                }

                await _creditoRepositorio.UpdateAsync(credito);

                var totalCobrado = abonoADepositar;
                var (montoEfectivo, montoTransferencia) = ResolverMontosPorMedio(medioPago, totalCobrado, request);

                var medioStored = medioPago.ToStoredString();
                var conceptoBase = esPagoCompleto ? $"Pago Ficha #{numeroFicha}" : $"Abono Ficha #{numeroFicha}";
                var concepto = $"{conceptoBase} [OP:{operacionKey}]";

                var movimiento = new MovimientoCaja
                {
                    Id = Guid.NewGuid(),
                    OperacionId = operacionId,
                    IdempotencyKey = idempotencyKey,
                    Tipo = TipoMovimientoCaja.Ficha.ToStoredString(),
                    Concepto = concepto,
                    Medio = medioStored,
                    Total = totalCobrado,
                    MontoEfectivo = montoEfectivo,
                    MontoTransferencia = montoTransferencia,
                    Abono = esPagoCompleto ? 0m : abonoADepositar,
                    Mora = 0,
                    CreditoId = credito.Id,
                    NumeroFicha = numeroFicha,
                    Fecha = _clock.Today,
                    Hora = _clock.Now.ToString("HH:mm"),
                    CobradorId = usuarioId,
                    RegistraCaja = true
                };

                await _movimientoCajaRepositorio.AddAsync(movimiento);

                await _auditoriaRepositorio.AddAsync(new Domain.Entidades.General.AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Accion = "CobroFicha",
                    EntidadTipo = "Credito",
                    EntidadId = credito.Id,
                    Fecha = _clock.UtcNow,
                    Detalle = $"Ficha:{numeroFicha};Total:{totalCobrado};Medio:{medioStored};Concepto:{concepto}"
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return MapCreditoConFichasOrdenadas(credito);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<CreditoDto> EjecutarMultiplesVigentes(Guid creditoId, AbonarFichaRequestDto request, Guid? usuarioId)
        {
            var operacionId = Guid.NewGuid();
            var operacionKey = Guid.NewGuid().ToString("N");
            var cantidadFichas = request?.CantidadFichas ?? 0;
            if (cantidadFichas <= 0) throw new BusinessRuleException("La cantidad de fichas debe ser mayor a 0");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var credito = await _creditoRepositorio.GetByIdAsync(creditoId);
                if (credito == null) throw new NotFoundException("No existe el crédito");

                await _creditoZonaAutorizacionService.AsegurarPuedeOperarAsync(credito, usuarioId);

                var hoy = _clock.Today.Date;
                var fichasVigentes = (credito.Fichas ?? Enumerable.Empty<Ficha>())
                    .Where(f => !f.Pagada)
                    .OrderBy(f => f.Fecha.Date < hoy ? 1 : 0)
                    .ThenBy(f => f.Num)
                    .ToList();

                if (!fichasVigentes.Any()) throw new BusinessRuleException("El crédito no tiene fichas vigentes");
                if (cantidadFichas > fichasVigentes.Count)
                    throw new BusinessRuleException("La cantidad de fichas excede las vigentes");

                var fichasAPagar = fichasVigentes.Take(cantidadFichas).ToList();
                var totalAPagar = fichasAPagar.Sum(f => Math.Max(0, (f.Capital + f.Interes + f.MoraAcumulada) - f.AbonoAcumulado));
                if (totalAPagar <= 0) throw new BusinessRuleException("No hay saldo pendiente en las fichas seleccionadas");

                var montoTotalRecibido = request?.MontoAbono ?? totalAPagar;
                if (montoTotalRecibido <= 0) throw new BusinessRuleException("El monto debe ser mayor a 0");
                if (Math.Abs(montoTotalRecibido - totalAPagar) > DecimalTolerance.Centavo)
                    throw new BusinessRuleException("El monto debe coincidir con el total a pagar de las fichas vigentes");

                var medioPago = ParseMedioPagoCobro(request?.Medio);
                var (montoEfectivoTotal, montoTransferenciaTotal) = ResolverMontosPorMedio(medioPago, montoTotalRecibido, request);
                var medioStored = medioPago.ToStoredString();

                var restanteEfectivo = montoEfectivoTotal ?? 0m;
                var restanteTransferencia = montoTransferenciaTotal ?? 0m;

                for (var i = 0; i < fichasAPagar.Count; i++)
                {
                    var ficha = fichasAPagar[i];
                    var adeudoFicha = Math.Max(0, (ficha.Capital + ficha.Interes + ficha.MoraAcumulada) - ficha.AbonoAcumulado);
                    if (adeudoFicha <= 0) continue;

                    ficha.Total = 0;
                    ficha.SaldoPendiente = 0;
                    ficha.Pagada = true;
                    ficha.FechaPago = _clock.Today;
                    ficha.Hora = _clock.Now.ToString("HH:mm");
                    ficha.Cerrada = true;
                    ficha.FechaCierre = _clock.UtcNow;

                    decimal movimientoEf;
                    decimal movimientoTr;
                    if (i == fichasAPagar.Count - 1)
                    {
                        movimientoEf = restanteEfectivo;
                        movimientoTr = restanteTransferencia;
                    }
                    else if (medioPago == MedioMovimientoCaja.Efectivo)
                    {
                        movimientoEf = adeudoFicha;
                        movimientoTr = 0m;
                    }
                    else if (medioPago == MedioMovimientoCaja.Transferencia)
                    {
                        movimientoEf = 0m;
                        movimientoTr = adeudoFicha;
                    }
                    else
                    {
                        var proporcion = adeudoFicha / montoTotalRecibido;
                        movimientoEf = Math.Round((montoEfectivoTotal ?? 0m) * proporcion, 2, MidpointRounding.AwayFromZero);
                        movimientoTr = adeudoFicha - movimientoEf;
                    }

                    restanteEfectivo -= movimientoEf;
                    restanteTransferencia -= movimientoTr;

                    await _movimientoCajaRepositorio.AddAsync(new MovimientoCaja
                    {
                        Id = Guid.NewGuid(),
                        OperacionId = operacionId,
                        IdempotencyKey = i == 0 ? request?.IdempotencyKey?.Trim() : null,
                        Tipo = TipoMovimientoCaja.Ficha.ToStoredString(),
                        Concepto = $"Pago Ficha #{ficha.Num} [OP:{operacionKey}]",
                        Medio = medioStored,
                        Total = adeudoFicha,
                        MontoEfectivo = movimientoEf,
                        MontoTransferencia = movimientoTr,
                        Abono = 0m,
                        Mora = 0,
                        CreditoId = credito.Id,
                        NumeroFicha = ficha.Num,
                        Fecha = _clock.Today,
                        Hora = _clock.Now.ToString("HH:mm"),
                        CobradorId = usuarioId,
                        RegistraCaja = true
                    });
                }

                credito.Pagado += montoTotalRecibido;
                if (credito.Fichas.All(f => f.Pagada))
                {
                    credito.Estatus = EstatusCredito.Liquidado.ToStoredString();
                }

                await _creditoRepositorio.UpdateAsync(credito);

                await _auditoriaRepositorio.AddAsync(new Domain.Entidades.General.AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Accion = "CobroFichasVigentes",
                    EntidadTipo = "Credito",
                    EntidadId = credito.Id,
                    Fecha = _clock.UtcNow,
                    Detalle = $"Cantidad:{cantidadFichas};Total:{montoTotalRecibido};Medio:{medioStored}"
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return MapCreditoConFichasOrdenadas(credito);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private CreditoDto MapCreditoConFichasOrdenadas(Credito credito)
        {
            var dto = _mapper.Map<CreditoDto>(credito);
            dto.Fichas = (credito.Fichas ?? Enumerable.Empty<Ficha>())
                .OrderBy(f => f.Num)
                .Select(f => _mapper.Map<FichaDto>(f))
                .ToList();
            return dto;
        }

        private static MedioMovimientoCaja ParseMedioPagoCobro(string? requestMedio)
        {
            var raw = string.IsNullOrWhiteSpace(requestMedio)
                ? MedioMovimientoCaja.Efectivo.ToStoredString()
                : requestMedio.Trim();

            if (!MedioMovimientoCajaExtensions.TryParseFromStored(raw, out var medio))
                throw new BusinessRuleException("Medio inválido");

            if (medio is not (MedioMovimientoCaja.Efectivo or MedioMovimientoCaja.Transferencia or MedioMovimientoCaja.Mixto))
                throw new BusinessRuleException("Medio inválido");

            return medio;
        }

        private static (decimal? montoEfectivo, decimal? montoTransferencia) ResolverMontosPorMedio(
            MedioMovimientoCaja medio,
            decimal totalCobrado,
            AbonarFichaRequestDto? request)
        {
            return medio switch
            {
                MedioMovimientoCaja.Mixto => ResolverMixto(totalCobrado, request),
                MedioMovimientoCaja.Efectivo => (totalCobrado, 0m),
                MedioMovimientoCaja.Transferencia => (0m, totalCobrado),
                _ => throw new BusinessRuleException("Medio inválido")
            };
        }

        private static (decimal? ef, decimal? tr) ResolverMixto(decimal totalCobrado, AbonarFichaRequestDto? request)
        {
            var ef = request?.MontoEfectivo ?? 0;
            var tr = request?.MontoTransferencia ?? 0;
            if (ef < 0 || tr < 0) throw new BusinessRuleException("Montos de pago inválidos");
            if (Math.Abs(ef + tr - totalCobrado) > DecimalTolerance.Centavo)
                throw new BusinessRuleException("Mixto debe sumar el total");
            return (ef, tr);
        }
    }
}
