using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Domain.Entidades.Finanzas;
using FinancieraSoluciones.Domain.Enums.Finanzas;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ReestructurarCreditoCasoUso
    {
        private readonly IMapper _mapper;
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IFeriadoRepositorio _feriadoRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ReestructurarCreditoCasoUso(
            IMapper mapper,
            ICreditoRepositorio creditoRepositorio,
            IConfiguracionSistemaRepositorio configuracionRepositorio,
            IFeriadoRepositorio feriadoRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _creditoRepositorio = creditoRepositorio;
            _configuracionRepositorio = configuracionRepositorio;
            _feriadoRepositorio = feriadoRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreditoDto> Ejecutar(Guid creditoId, ReestructurarCreditoRequestDto request, Guid? usuarioId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var credito = await _creditoRepositorio.GetByIdAsync(creditoId);
                if (credito == null) throw new ArgumentException("No existe el crédito");
                if (!EstatusCreditoExtensions.EqualsStored(credito.Estatus, EstatusCredito.Activo))
                    throw new InvalidOperationException("Solo se puede reestructurar un crédito vigente (Activo).");
                if (request.NuevoMonto <= 0) throw new ArgumentException("NuevoMonto debe ser mayor a 0");
                if (request.NuevoPlazo <= 0) throw new ArgumentException("NuevoPlazo debe ser mayor a 0");
                if (string.IsNullOrWhiteSpace(request.Tipo)) throw new ArgumentException("Tipo es requerido");

                var config = await _configuracionRepositorio.GetAsync();
                if (config == null) throw new ArgumentException("No existe configuración del sistema");

                var tipo = request.Tipo.Trim().ToLower();
                if (tipo != "diario" && tipo != "semanal" && tipo != "mensual") throw new ArgumentException("Tipo inválido");

                var tasa = tipo == "diario" ? config.TasaDiaria : (tipo == "semanal" ? config.TasaSemanal : config.TasaMensual);
                var interesTotal = Math.Round(request.NuevoMonto * tasa, 2);
                var total = request.NuevoMonto + interesTotal;
                var cuota = Math.Ceiling(total / request.NuevoPlazo);
                var capitalPorFicha = Math.Round(request.NuevoMonto / request.NuevoPlazo, 2);
                var interesPorFicha = Math.Round(interesTotal / request.NuevoPlazo, 2);

                credito.Estatus = EstatusCredito.Reestructurado.ToStoredString();
                await _creditoRepositorio.UpdateAsync(credito);

                var nuevoCreditoId = Guid.NewGuid();
                var folioCredito = await _creditoRepositorio.ObtenerSiguienteFolioAsync(DateTime.Today);
                var permitirDomingo = credito.PermitirDomingo;
                var aplicarFeriados = credito.AplicarFeriados;
                var fechaBase = DateTime.Today;

                var multiplier = tipo == "diario" ? 1 : (tipo == "semanal" ? 7 : 30);
                var maxDias = request.NuevoPlazo * multiplier + 60;
                var feriados = aplicarFeriados
                    ? (await _feriadoRepositorio.GetActivosEnRangoAsync(fechaBase, fechaBase.AddDays(maxDias)))
                        .Select(f => f.Fecha.Date)
                        .ToHashSet()
                    : new HashSet<DateTime>();

                var nuevoCredito = new Credito
                {
                    Id = nuevoCreditoId,
                    ClienteId = credito.ClienteId,
                    Folio = folioCredito,
                    Monto = request.NuevoMonto,
                    InteresTotal = interesTotal,
                    Total = total,
                    Cuota = cuota,
                    TotalFichas = request.NuevoPlazo,
                    Pagado = 0,
                    Tipo = tipo,
                    Estatus = EstatusCredito.Activo.ToStoredString(),
                    FechaCreacion = DateTime.UtcNow,
                    PermitirDomingo = permitirDomingo,
                    AplicarFeriados = aplicarFeriados,
                    Fichas = BuildFichas(nuevoCreditoId, folioCredito, tipo, request.NuevoMonto, request.NuevoPlazo, cuota, capitalPorFicha, interesPorFicha, fechaBase, permitirDomingo, aplicarFeriados, feriados)
                };

                var created = await _creditoRepositorio.AddAsync(nuevoCredito);

                await _auditoriaRepositorio.AddAsync(new Domain.Entidades.General.AuditoriaEvento
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuarioId,
                    Accion = "ReestructurarCredito",
                    EntidadTipo = "Credito",
                    EntidadId = creditoId,
                    Fecha = DateTime.UtcNow,
                    Detalle = $"NuevoCredito:{nuevoCreditoId};NuevoMonto:{request.NuevoMonto};NuevoPlazo:{request.NuevoPlazo};Tipo:{tipo}"
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                var dto = _mapper.Map<CreditoDto>(created);
                dto.ClienteNombre = credito.Cliente?.Nombre ?? string.Empty;
                dto.ClienteApellido = credito.Cliente?.Apellido ?? string.Empty;
                dto.ClienteNegocio = credito.Cliente?.Negocio ?? string.Empty;
                dto.ClienteZona = credito.Cliente?.Zona ?? string.Empty;
                dto.Fichas = created.Fichas
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

        private static List<Ficha> BuildFichas(
            Guid creditoId,
            string creditoFolio,
            string tipo,
            decimal monto,
            int plazo,
            decimal cuota,
            decimal capitalPorFicha,
            decimal interesPorFicha,
            DateTime fechaBase,
            bool permitirDomingo,
            bool aplicarFeriados,
            HashSet<DateTime> feriados)
        {
            var fichas = new List<Ficha>();
            var fecha = fechaBase.Date;
            for (var i = 0; i < plazo; i++)
            {
                if (tipo == "diario") fecha = fecha.AddDays(1);
                else if (tipo == "semanal") fecha = fecha.AddDays(7);
                else fecha = fecha.AddMonths(1);
                fecha = AjustarFecha(fecha, permitirDomingo, aplicarFeriados, feriados);
                fichas.Add(new Ficha
                {
                    Id = Guid.NewGuid(),
                    CreditoId = creditoId,
                    Num = i + 1,
                    Fecha = fecha,
                    Folio = $"{creditoFolio}-{i + 1:000}",
                    Capital = capitalPorFicha,
                    Interes = interesPorFicha,
                    Total = capitalPorFicha + interesPorFicha,
                    AbonoAcumulado = 0,
                    MoraAcumulada = 0,
                    SaldoCap = Math.Max(0, monto - capitalPorFicha * i),
                    SaldoPendiente = capitalPorFicha + interesPorFicha,
                    Pagada = false
                });
            }
            return fichas;
        }

        private static DateTime AjustarFecha(DateTime fecha, bool permitirDomingo, bool aplicarFeriados, HashSet<DateTime> feriados)
        {
            var f = fecha.Date;
            for (var i = 0; i < 31; i++)
            {
                var isDomingo = f.DayOfWeek == DayOfWeek.Sunday;
                var isFeriado = aplicarFeriados && feriados != null && feriados.Contains(f);
                if ((!isDomingo || permitirDomingo) && !isFeriado) return f;
                f = f.AddDays(1);
            }
            return f;
        }
    }
}
