using System;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ActualizarMoraAcumuladaCasoUso
    {
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IConfiguracionSistemaRepositorio _configuracionRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarMoraAcumuladaCasoUso(
            ICreditoRepositorio creditoRepositorio,
            IConfiguracionSistemaRepositorio configuracionRepositorio,
            IUnitOfWork unitOfWork)
        {
            _creditoRepositorio = creditoRepositorio;
            _configuracionRepositorio = configuracionRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Ejecutar(DateTime hoy)
        {
            var config = await _configuracionRepositorio.GetAsync();
            if (config == null) throw new ArgumentException("No existe configuración del sistema");

            var fichas = (await _creditoRepositorio.GetFichasPendientesParaMoraAsync(hoy)).ToList();
            var updated = 0;
            foreach (var ficha in fichas)
            {
                var tipo = ficha.Credito?.Tipo ?? string.Empty;
                var mora = MoraCalculator.Calcular(
                    tipo,
                    ficha.Fecha.Date,
                    hoy.Date,
                    config.MoraDiaria,
                    config.MoraSemanal,
                    config.MoraMensual,
                    config.DiasGraciaDiaria,
                    config.DiasGraciaSemanal,
                    config.DiasGraciaMensual,
                    config.TopeMoraDiaria,
                    config.TopeMoraSemanal,
                    config.TopeMoraMensual);

                if (mora < ficha.MoraAcumulada) mora = ficha.MoraAcumulada;
                if (ficha.MoraAcumulada != mora)
                {
                    ficha.MoraAcumulada = mora;
                    
                    // Recalcular saldos concentradores
                    ficha.Total = (ficha.Capital + ficha.Interes + ficha.MoraAcumulada) - ficha.AbonoAcumulado;
                    ficha.SaldoPendiente = ficha.Total;
                    if (ficha.Total < 0) { ficha.Total = 0; ficha.SaldoPendiente = 0; }
                    
                    updated++;
                }
            }

            if (updated > 0) await _unitOfWork.SaveChangesAsync();
            return updated;
        }
    }
}

