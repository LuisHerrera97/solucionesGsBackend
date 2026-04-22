using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ObtenerPermisosPorPerfilCasoUso
    {
        private readonly IPermisoModuloRepositorio _permisoModuloRepositorio;
        private readonly IPermisoPaginaRepositorio _permisoPaginaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public ObtenerPermisosPorPerfilCasoUso(
            IPermisoModuloRepositorio permisoModuloRepositorio,
            IPermisoPaginaRepositorio permisoPaginaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _permisoModuloRepositorio = permisoModuloRepositorio;
            _permisoPaginaRepositorio = permisoPaginaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        public async Task<AsignarPermisosRequestDto> Ejecutar(Guid idPerfil)
        {
            var permisosModulos = await _permisoModuloRepositorio.GetByPerfilIdAsync(idPerfil);
            
            var permisosPaginas = await _permisoPaginaRepositorio.GetByPerfilIdAsync(idPerfil);
            
            var permisosBotones = await _permisoBotonRepositorio.GetByPerfilIdAsync(idPerfil);

            var permisosDto = new AsignarPermisosRequestDto
            {
                IdPerfil = idPerfil,
                ModulosPermitidos = permisosModulos
                    .Where(pm => pm.TienePermiso)
                    .Select(pm => pm.IdModulo)
                    .ToList(),
                PaginasPermitidas = permisosPaginas
                    .Where(pp => pp.TienePermiso)
                    .Select(pp => pp.IdPagina)
                    .ToList(),
                BotonesPermitidos = permisosBotones
                    .Where(pb => pb.TienePermiso)
                    .Select(pb => pb.IdBoton)
                    .ToList()
            };

            return permisosDto;
        }
    }
}
