using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Domain.Entidades.Finanzas;
using FinancieraSoluciones.Domain.Enums.Finanzas;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class CrearCreditoCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IFeriadoRepositorio _feriadoRepositorio;
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;

        public CrearCreditoCasoUso(
            IMapper mapper,
            IClienteRepositorio clienteRepositorio,
            ICreditoRepositorio creditoRepositorio,
            IConfiguracionSistemaRepositorio configuracionRepositorio,
            IFeriadoRepositorio feriadoRepositorio,
            IAuditoriaEventoRepositorio auditoriaRepositorio,
            IClock clock,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _clienteRepositorio = clienteRepositorio;
            _creditoRepositorio = creditoRepositorio;
            _configuracionRepositorio = configuracionRepositorio;
            _feriadoRepositorio = feriadoRepositorio;
            _auditoriaRepositorio = auditoriaRepositorio;
            _clock = clock;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreditoDto> Ejecutar(CrearCreditoRequestDto request, Guid? usuarioId)
        {
            if (request.ClienteId == Guid.Empty) throw new ArgumentException("ClienteId es requerido");
            if (request.Monto <= 0) throw new ArgumentException("Monto debe ser mayor a 0");
            if (request.Plazo <= 0) throw new ArgumentException("Plazo debe ser mayor a 0");
            if (string.IsNullOrWhiteSpace(request.Tipo)) throw new ArgumentException("Tipo es requerido");

            var cliente = await _clienteRepositorio.GetByIdAsync(request.ClienteId);
            if (cliente == null) throw new ArgumentException("No existe el cliente");

            var config = await _configuracionRepositorio.GetAsync();
            if (config == null) throw new ArgumentException("No existe configuración del sistema");

            var tipo = request.Tipo.Trim().ToLower();
            if (tipo != "diario" && tipo != "semanal" && tipo != "mensual") throw new ArgumentException("Tipo inválido");

            var tasaDefault = tipo == "diario" ? config.TasaDiaria : (tipo == "semanal" ? config.TasaSemanal : config.TasaMensual);
            var tasa = request.TasaManual ?? tasaDefault;
            var interesTotal = Math.Round(request.Monto * tasa, 2);
            var total = request.Monto + interesTotal;
            var cuota = Math.Ceiling(total / request.Plazo);
            var capitalPorFicha = Math.Round(request.Monto / request.Plazo, 2);
            var interesPorFicha = Math.Round(interesTotal / request.Plazo, 2);

            var creditoId = Guid.NewGuid();
            var fechaBase = DateTime.Today;
            var folioCredito = await _creditoRepositorio.ObtenerSiguienteFolioAsync(DateTime.Today);
            var permitirDomingo = request.PermitirDomingo ?? !config.DomingoInhabilDefault;
            var aplicarFeriados = request.AplicarFeriados ?? config.AplicarFeriadosDefault;

            var multiplier = tipo == "diario" ? 1 : (tipo == "semanal" ? 7 : 30);
            var maxDias = request.Plazo * multiplier + 60;
            var feriados = aplicarFeriados
                ? (await _feriadoRepositorio.GetActivosEnRangoAsync(fechaBase, fechaBase.AddDays(maxDias)))
                    .Select(f => f.Fecha.Date)
                    .ToHashSet()
                : new HashSet<DateTime>();

            var fichas = BuildFichas(
                creditoId,
                folioCredito,
                tipo,
                request.Monto,
                request.Plazo,
                cuota,
                capitalPorFicha,
                interesPorFicha,
                fechaBase,
                permitirDomingo,
                aplicarFeriados,
                feriados);

            var credito = new Credito
            {
                Id = creditoId,
                ClienteId = cliente.Id,
                Folio = folioCredito,
                Monto = request.Monto,
                InteresTotal = interesTotal,
                Total = total,
                Cuota = cuota,
                TotalFichas = request.Plazo,
                Pagado = 0,
                Tipo = tipo,
                Estatus = EstatusCredito.Activo.ToStoredString(),
                FechaCreacion = DateTime.UtcNow,
                PermitirDomingo = permitirDomingo,
                AplicarFeriados = aplicarFeriados,
                Observacion = request.Observacion,
                Fichas = fichas
            };

            var created = await _creditoRepositorio.AddAsync(credito);

            await _auditoriaRepositorio.AddAsync(new AuditoriaEvento
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Accion = "CrearCredito",
                EntidadTipo = "Credito",
                EntidadId = created.Id,
                Fecha = _clock.UtcNow,
                Detalle = $"Folio:{created.Folio};ClienteId:{created.ClienteId};Monto:{created.Monto};Plazo:{created.TotalFichas};Tipo:{created.Tipo}"
            });

            await _unitOfWork.SaveChangesAsync();

            return MapCredito(created, cliente);
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

        private CreditoDto MapCredito(Credito credito, Cliente cliente)
        {
            var dto = _mapper.Map<CreditoDto>(credito);
            dto.ClienteNombre = cliente?.Nombre ?? string.Empty;
            dto.ClienteApellido = cliente?.Apellido ?? string.Empty;
            dto.ClienteNegocio = cliente?.Negocio ?? string.Empty;
            dto.ClienteZona = cliente?.Zona ?? string.Empty;
            dto.Fichas = (credito.Fichas ?? new List<Ficha>())
                .OrderBy(f => f.Num)
                .Select(f => _mapper.Map<FichaDto>(f))
                .ToList();
            return dto;
        }
    }
}
