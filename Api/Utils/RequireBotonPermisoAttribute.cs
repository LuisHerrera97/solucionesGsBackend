using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Shared;
using FinancieraSoluciones.Domain.Interfaces.Seguridad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinancieraSoluciones.Api.Utils
{
    public sealed class RequireBotonPermisoAttribute : TypeFilterAttribute
    {
        public RequireBotonPermisoAttribute(string botonClave) : base(typeof(RequireBotonPermisoFilter))
        {
            Arguments = new object[] { botonClave };
        }
    }

    public sealed class RequireBotonPermisoFilter : IAsyncActionFilter
    {
        private readonly string _botonClave;
        private readonly IPerfilRepositorio _perfilRepositorio;
        private readonly IPermisoBotonRepositorio _permisoBotonRepositorio;

        public RequireBotonPermisoFilter(string botonClave, IPerfilRepositorio perfilRepositorio, IPermisoBotonRepositorio permisoBotonRepositorio)
        {
            _botonClave = botonClave;
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

            var tienePermiso = await _permisoBotonRepositorio.HasPermisoAsync(perfilId.Value, _botonClave);
            if (!tienePermiso)
            {
                context.Result = new OkObjectResult(ApiResponse<object>.Fail("No tienes permisos para realizar esta acción", 403));
                return;
            }

            await next();
        }
    }
}
