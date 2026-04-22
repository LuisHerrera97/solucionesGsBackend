using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General
{
    public class EliminarZonaCobranzaCasoUso
    {
        private readonly IZonaCobranzaRepositorio _zonaRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public EliminarZonaCobranzaCasoUso(IZonaCobranzaRepositorio zonaRepositorio, IUnitOfWork unitOfWork)
        {
            _zonaRepositorio = zonaRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Ejecutar(Guid id)
        {
            var zona = await _zonaRepositorio.GetByIdAsync(id);
            if (zona == null)
            {
                return false;
            }

            await _zonaRepositorio.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
