using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Application.Servicios.Finanzas;
using FinancieraSoluciones.Domain.Common;
using FinancieraSoluciones.Domain.Entidades.Finanzas.Caja;
using FinancieraSoluciones.Domain.Enums.Finanzas;
using FinancieraSoluciones.Domain.Enums.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas.Caja;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ReversarOperacionMovimientoCasoUso
    {
        private static readonly Regex OperacionRegex = new(@"\[OP:(?<op>[^\]]+)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly IMapper _mapper;
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IMovimientoCajaRepositorio _movimientoCajaRepositorio;
        private readonly ICreditoZonaAutorizacionService _creditoZonaAutorizacionService;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public ReversarOperacionMovimientoCasoUso(
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

        public async Task<CreditoDto> Ejecutar(Guid creditoId, Guid movimientoId, Guid? usuarioId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var credito = await _creditoRepositorio.GetByIdAsync(creditoId);
                if (credito == null) throw new NotFoundException("No existe el crédito");
                await _creditoZonaAutorizacionService.AsegurarPuedeOperarAsync(credito, usuarioId);

                var movimientos = (await _movimientoCajaRepositorio.ObtenerPorCreditoAsync(creditoId)).ToList();
                var movimientoRaiz = movimientos.FirstOrDefault(m => m.Id == movimientoId);
                if (movimientoRaiz == null) throw new NotFoundException("No existe el movimiento");
                if (movimientoRaiz.ReversaDeId.HasValue) throw new BusinessRuleException("No puedes desaplicar un movimiento de reversa");

                var operacionKey = ExtraerOperacionKey(movimientoRaiz.Concepto);
                var operacion = (await _movimientoCajaRepositorio.ObtenerPorOperacionAsync(
                    creditoId,
                    movimientoRaiz.OperacionId,
                    operacionKey)).ToList();
                if (operacion.Count == 0)
                {
                    operacion = new List<MovimientoCaja> { movimientoRaiz };
                }

                var operacionIds = operacion.Select(x => x.Id).ToHashSet();
                var yaRevertidos = movimientos.Where(m => m.ReversaDeId.HasValue && operacionIds.Contains(m.ReversaDeId.Value)).ToList();
                if (yaRevertidos.Count > 0) throw new BusinessRuleException("La operación ya fue revertida");

                foreach (var mov in operacion.OrderByDescending(x => x.Fecha).ThenByDescending(x => x.Hora))
                {
                    var ficha = mov.NumeroFicha.HasValue
                        ? credito.Fichas.FirstOrDefault(f => f.Num == mov.NumeroFicha.Value)
                        : null;

                    if (mov.Tipo == TipoMovimientoCaja.Ficha.ToStoredString() && ficha != null)
                    {
                        var esPagoFicha = (mov.Concepto ?? string.Empty).StartsWith("Pago Ficha", StringComparison.OrdinalIgnoreCase);
                        if (esPagoFicha)
                        {
                            ficha.Pagada = false;
                            ficha.FechaPago = null;
                            ficha.Hora = null;
                            ficha.Cerrada = false;
                            ficha.FechaCierre = null;
                            ficha.Total = Math.Max(0, (ficha.Capital + ficha.Interes + ficha.MoraAcumulada) - ficha.AbonoAcumulado);
                            ficha.SaldoPendiente = ficha.Total;
                            credito.Pagado = Math.Max(0, credito.Pagado - mov.Total);
                        }
                        else
                        {
                            var abono = mov.Abono ?? 0m;
                            ficha.AbonoAcumulado = Math.Max(0, ficha.AbonoAcumulado - abono);
                            RecalcularFicha(ficha);
                            credito.Pagado = Math.Max(0, credito.Pagado - abono);
                        }
                    }
                    else if (mov.Tipo == TipoMovimientoCaja.Penalizacion.ToStoredString() && ficha != null)
                    {
                        var mora = mov.Mora ?? 0m;
                        ficha.MoraAcumulada = Math.Max(0, ficha.MoraAcumulada - mora);
                        RecalcularFicha(ficha);
                    }

                    var totalReversa = mov.Total < 0 ? Math.Abs(mov.Total) : -mov.Total;
                    var reversa = new MovimientoCaja
                    {
                        Id = Guid.NewGuid(),
                        OperacionId = mov.OperacionId,
                        Tipo = mov.Tipo,
                        Concepto = $"Reversa de {mov.Concepto}",
                        Medio = mov.Medio,
                        Total = totalReversa,
                        MontoEfectivo = mov.MontoEfectivo.HasValue ? -mov.MontoEfectivo.Value : null,
                        MontoTransferencia = mov.MontoTransferencia.HasValue ? -mov.MontoTransferencia.Value : null,
                        Abono = mov.Abono.HasValue ? -mov.Abono.Value : null,
                        Mora = mov.Mora.HasValue ? -mov.Mora.Value : null,
                        CreditoId = mov.CreditoId,
                        NumeroFicha = mov.NumeroFicha,
                        Fecha = _clock.Today,
                        Hora = _clock.Now.ToString("HH:mm"),
                        CobradorId = usuarioId,
                        RegistraCaja = mov.RegistraCaja,
                        ReversaDeId = mov.Id
                    };
                    await _movimientoCajaRepositorio.AddAsync(reversa);
                }

                credito.Estatus = credito.Fichas.All(f => f.Pagada) ? EstatusCredito.Liquidado.ToStoredString() : EstatusCredito.Activo.ToStoredString();
                await _creditoRepositorio.UpdateAsync(credito);

                await _auditoriaRepositorio.AddAsync(new Domain.Entidades.General.AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Accion = "ReversaOperacionCobro",
                    EntidadTipo = "Credito",
                    EntidadId = credito.Id,
                    Fecha = _clock.UtcNow,
                    Detalle = $"Movimiento:{movimientoId};Operacion:{operacionKey ?? movimientoId.ToString()}"
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var dto = _mapper.Map<CreditoDto>(credito);
                dto.Fichas = credito.Fichas.OrderBy(f => f.Num).Select(f => _mapper.Map<FichaDto>(f)).ToList();
                return dto;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        private static string? ExtraerOperacionKey(string? concepto)
        {
            if (string.IsNullOrWhiteSpace(concepto)) return null;
            var match = OperacionRegex.Match(concepto);
            return match.Success ? match.Groups["op"].Value.Trim() : null;
        }

        private static void RecalcularFicha(Domain.Entidades.Finanzas.Ficha ficha)
        {
            var total = (ficha.Capital + ficha.Interes + ficha.MoraAcumulada) - ficha.AbonoAcumulado;
            ficha.Total = total < 0 ? 0 : total;
            ficha.SaldoPendiente = ficha.Total;
            ficha.Pagada = ficha.Total <= 0;
            if (!ficha.Pagada)
            {
                ficha.FechaPago = null;
                ficha.Hora = null;
                ficha.Cerrada = false;
                ficha.FechaCierre = null;
            }
        }
    }
}
