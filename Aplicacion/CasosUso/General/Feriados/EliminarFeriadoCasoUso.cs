using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General.Feriados
{
    public class EliminarFeriadoCasoUso
    {
        private readonly IFeriadoRepositorio _feriadoRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public EliminarFeriadoCasoUso(IFeriadoRepositorio feriadoRepositorio, IUnitOfWork unitOfWork)
        {
            _feriadoRepositorio = feriadoRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task Ejecutar(Guid id)
        {
            await _feriadoRepositorio.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

