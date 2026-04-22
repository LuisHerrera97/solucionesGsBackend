using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class EliminarPerfilCasoUso
    {
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public EliminarPerfilCasoUso(IPerfilRepositorio perfilRepositorio, IUnitOfWork unitOfWork)
        {
            _perfilRepositorio = perfilRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Ejecutar(Guid idPerfil)
        {
            var perfil = await _perfilRepositorio.GetByIdAsync(idPerfil);
            if (perfil == null)
            {
                return false;
            }

            await _perfilRepositorio.DeleteAsync(idPerfil);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
