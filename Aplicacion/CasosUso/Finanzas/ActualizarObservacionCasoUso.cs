using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ActualizarObservacionCasoUso
    {
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public ActualizarObservacionCasoUso(ICreditoRepositorio creditoRepositorio, IUnitOfWork unitOfWork)
        {
            _creditoRepositorio = creditoRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task Ejecutar(Guid creditoId, string observacion)
        {
            var credito = await _creditoRepositorio.GetByIdAsync(creditoId);
            if (credito == null) throw new ArgumentException("Crédito no encontrado");

            credito.Observacion = string.IsNullOrWhiteSpace(observacion) ? null : observacion;

            await _creditoRepositorio.UpdateAsync(credito);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
