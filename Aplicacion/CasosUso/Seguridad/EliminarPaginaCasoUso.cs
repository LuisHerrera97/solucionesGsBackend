using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class EliminarPaginaCasoUso
    {
        private readonly IPaginaRepositorio _paginaRepositorio;
        private readonly IPermisoPaginaRepositorio _permisoPaginaRepositorio;
        private readonly IBotonRepositorio _botonRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public EliminarPaginaCasoUso(
            IPaginaRepositorio paginaRepositorio,
            IPermisoPaginaRepositorio permisoPaginaRepositorio,
            IBotonRepositorio botonRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio,
            IUnitOfWork unitOfWork)
        {
            _paginaRepositorio = paginaRepositorio;
            _permisoPaginaRepositorio = permisoPaginaRepositorio;
            _botonRepositorio = botonRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Ejecutar(Guid idPagina)
        {
            var pagina = await _paginaRepositorio.GetByIdAsync(idPagina);
            if (pagina == null)
            {
                return false;
            }

            // 1. Eliminar permisos de la página
            await _permisoPaginaRepositorio.DeleteByPaginaIdAsync(idPagina);

            // 2. Eliminar botones y sus permisos
            var botones = await _botonRepositorio.GetByPaginaIdAsync(idPagina);
            foreach (var b in botones)
            {
                var todosPermisosB = await _permisoBotonRepositorio.GetAllAsync();
                foreach (var pb in todosPermisosB)
                {
                    if (pb.IdBoton == b.Id)
                    {
                        await _permisoBotonRepositorio.DeleteAsync(pb.Id);
                    }
                }
                await _botonRepositorio.DeleteAsync(b.Id);
            }

            // 3. Eliminar la página
            await _paginaRepositorio.DeleteAsync(idPagina);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
