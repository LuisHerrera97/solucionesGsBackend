using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class ObtenerModulosConPermisosCasoUso
    {
        private readonly IMapper _mapper;
        private readonly IModuloRepositorio _moduloRepositorio;
        private readonly IPaginaRepositorio _paginaRepositorio;
        private readonly IBotonRepositorio _botonRepositorio;
        private readonly IPermisoModuloRepositorio _permisoModuloRepositorio;
        private readonly IPermisoPaginaRepositorio _permisoPaginaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public ObtenerModulosConPermisosCasoUso(
            IMapper mapper,
            IModuloRepositorio moduloRepositorio,
            IPaginaRepositorio paginaRepositorio,
            IBotonRepositorio botonRepositorio,
            IPermisoModuloRepositorio permisoModuloRepositorio,
            IPermisoPaginaRepositorio permisoPaginaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _mapper = mapper;
            _moduloRepositorio = moduloRepositorio;
            _paginaRepositorio = paginaRepositorio;
            _botonRepositorio = botonRepositorio;
            _permisoModuloRepositorio = permisoModuloRepositorio;
            _permisoPaginaRepositorio = permisoPaginaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        public async Task<IEnumerable<ModuloDto>> Ejecutar(Guid idPerfil)
        {
            var modulos = await _moduloRepositorio.GetAllAsync();
            
            var paginas = await _paginaRepositorio.GetAllAsync();
            
            var botones = await _botonRepositorio.GetAllAsync();
            
            var permisosModulos = await _permisoModuloRepositorio.GetByPerfilIdAsync(idPerfil);
            var permisosModulosDict = permisosModulos
                .GroupBy(pm => pm.IdModulo)
                .ToDictionary(g => g.Key, g => g.Any(x => x.TienePermiso));
            
            var permisosPaginas = await _permisoPaginaRepositorio.GetByPerfilIdAsync(idPerfil);
            var permisosPaginasDict = permisosPaginas
                .GroupBy(pp => pp.IdPagina)
                .ToDictionary(g => g.Key, g => g.Any(x => x.TienePermiso));
            
            var permisosBotones = await _permisoBotonRepositorio.GetByPerfilIdAsync(idPerfil);
            var permisosBotonesDict = permisosBotones
                .GroupBy(pb => pb.IdBoton)
                .ToDictionary(g => g.Key, g => g.Any(x => x.TienePermiso));

            var modulosDto = modulos.Select(m =>
            {
                var moduloDto = _mapper.Map<ModuloDto>(m);
                moduloDto.TienePermiso = permisosModulosDict.TryGetValue(m.Id, out var mp) && mp;
                moduloDto.Paginas = paginas
                    .Where(p => p.IdModulo == m.Id)
                    .OrderBy(p => p.Orden)
                    .Select(p =>
                    {
                        var paginaDto = _mapper.Map<PaginaDto>(p);
                        paginaDto.NombreModulo = m.Nombre;
                        paginaDto.TienePermiso = permisosPaginasDict.TryGetValue(p.Id, out var pp) && pp;
                        paginaDto.Botones = botones
                            .Where(b => b.IdPagina == p.Id)
                            .OrderBy(b => b.Orden)
                            .Select(b =>
                            {
                                var botonDto = _mapper.Map<BotonDto>(b);
                                botonDto.NombrePagina = p.Nombre;
                                botonDto.TienePermiso = permisosBotonesDict.TryGetValue(b.Id, out var bp) && bp;
                                return botonDto;
                            })
                            .ToList();
                        return paginaDto;
                    })
                    .ToList();
                return moduloDto;
            }).ToList();

            return modulosDto.OrderBy(m => m.Orden);
        }
    }
}
