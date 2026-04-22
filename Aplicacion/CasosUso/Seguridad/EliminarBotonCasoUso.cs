using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class EliminarBotonCasoUso
    {
        private readonly IBotonRepositorio _botonRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public EliminarBotonCasoUso(IBotonRepositorio botonRepositorio, IUnitOfWork unitOfWork)
        {
            _botonRepositorio = botonRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Ejecutar(Guid idBoton)
        {
            var boton = await _botonRepositorio.GetByIdAsync(idBoton);
            if (boton == null)
            {
                return false;
            }

            await _botonRepositorio.DeleteAsync(idBoton);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
