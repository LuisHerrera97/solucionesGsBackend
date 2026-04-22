using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinancieraSoluciones.Api.Utils
{
    /// <summary>
    /// Exige que el perfil tenga permiso sobre al menos una de las claves de botón indicadas.
    /// </summary>
    public sealed class RequireBotonPermisoAlgunoAttribute : TypeFilterAttribute
    {
        public RequireBotonPermisoAlgunoAttribute(params string[] botonesClaves) : base(typeof(RequireBotonPermisoAlgunoFilter))
        {
            Arguments = new object[] { botonesClaves };
        }
    }

    public sealed class RequireBotonPermisoAlgunoFilter : IAsyncActionFilter
    {
        private readonly string[] _botonesClaves;
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public RequireBotonPermisoAlgunoFilter(string[] botonesClaves, IPerfilRepositorio perfilRepositorio, IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _botonesClaves = botonesClaves ?? Array.Empty<string>();
            _perfilRepositorio = perfilRepositorio;
            _permisoBotonRepositorio = permisoBotonRepositorio;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var perfilId = CurrentUser.GetPerfilId(context.HttpContext.User);
            if (!perfilId.HasValue)
            {
                context.Result = new OkObjectResult(ApiResponse<object>.Fail("Usuario no autenticado", 401));
                return;
            }

            var perfil = await _perfilRepositorio.GetByIdAsync(perfilId.Value);
            if (perfil != null && string.Equals(perfil.Clave, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                await next();
                return;
            }

            var tienePermiso = await _permisoBotonRepositorio.HasPermisoAlgunoAsync(perfilId.Value, _botonesClaves);
            if (!tienePermiso)
            {
                context.Result = new OkObjectResult(ApiResponse<object>.Fail("No tienes permisos para realizar esta acción", 403));
                return;
            }

            await next();
        }
    }
}
