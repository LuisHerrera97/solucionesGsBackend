using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class EliminarUsuarioCasoUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public EliminarUsuarioCasoUso(IUsuarioRepositorio usuarioRepositorio, IUnitOfWork unitOfWork)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Ejecutar(Guid id)
        {
            var usuarioExistente = await _usuarioRepositorio.GetByIdAsync(id);
            if (usuarioExistente == null)
            {
                return false;
            }

            await _usuarioRepositorio.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
