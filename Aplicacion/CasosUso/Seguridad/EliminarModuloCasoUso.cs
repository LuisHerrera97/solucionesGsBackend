using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class EliminarModuloCasoUso
    {
        private readonly IModuloRepositorio _moduloRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public EliminarModuloCasoUso(IModuloRepositorio moduloRepositorio, IUnitOfWork unitOfWork)
        {
            _moduloRepositorio = moduloRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Ejecutar(Guid idModulo)
        {
            var modulo = await _moduloRepositorio.GetByIdAsync(idModulo);
            if (modulo == null)
            {
                return false;
            }

            await _moduloRepositorio.DeleteAsync(idModulo);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
