using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Seguridad;
using FinancieraSoluciones.Domain.Entidades.Seguridad;
using FinancieraSoluciones.Domain.Interfaces;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;

namespace FinancieraSoluciones.Application.CasosUso.Seguridad
{
    public class AsignarPermisosCasoUso
    {
        private readonly IPermisoModuloRepositorio _permisoModuloRepositorio;
        private readonly IPermisoPaginaRepositorio _permisoPaginaRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;
        private readonly IUnitOfWork _unitOfWork;

        public AsignarPermisosCasoUso(
            IPermisoModuloRepositorio permisoModuloRepositorio,
            IPermisoPaginaRepositorio permisoPaginaRepositorio,
            IPermisoBotonRepositorio permisoBotonRepositorio,
            IUnitOfWork unitOfWork)
        {
            _permisoModuloRepositorio = permisoModuloRepositorio;
            _permisoPaginaRepositorio = permisoPaginaRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Ejecutar(AsignarPermisosRequestDto request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    await _permisoModuloRepositorio.DeleteByPerfilIdAsync(request.IdPerfil);
                    await _permisoPaginaRepositorio.DeleteByPerfilIdAsync(request.IdPerfil);
                    await _permisoBotonRepositorio.DeleteByPerfilIdAsync(request.IdPerfil);

                    foreach (var moduloId in request.ModulosPermitidos)
                    {
                        var permisoModulo = new PermisoModulo
                        {
                            Id = Guid.NewGuid(),
                            IdPerfil = request.IdPerfil,
                            IdModulo = moduloId,
                            TienePermiso = true,
                            FechaAsignacion = DateTime.UtcNow
                        };

                        await _permisoModuloRepositorio.AddAsync(permisoModulo);
                    }

                    foreach (var paginaId in request.PaginasPermitidas)
                    {
                        var permisoPagina = new PermisoPagina
                        {
                            Id = Guid.NewGuid(),
                            IdPerfil = request.IdPerfil,
                            IdPagina = paginaId,
                            TienePermiso = true,
                            FechaAsignacion = DateTime.UtcNow
                        };

                        await _permisoPaginaRepositorio.AddAsync(permisoPagina);
                    }

                    foreach (var botonId in request.BotonesPermitidos)
                    {
                        var permisoBoton = new PermisoBoton
                        {
                            Id = Guid.NewGuid(),
                            IdPerfil = request.IdPerfil,
                            IdBoton = botonId,
                            TienePermiso = true,
                            FechaAsignacion = DateTime.UtcNow
                        };

                        await _permisoBotonRepositorio.AddAsync(permisoBoton);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
