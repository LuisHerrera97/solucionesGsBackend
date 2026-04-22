using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinancieraSoluciones.Api.Utils
{
    public static class CurrentUser
    {
        /// <summary>
        /// Obtiene el id del usuario del JWT. Incluye <see cref="ClaimTypes.NameIdentifier"/> porque el handler
        /// de JwtBearer suele mapear el claim <c>sub</c> a ese tipo al entrar.
        /// </summary>
        public static Guid? GetUserId(ClaimsPrincipal user)
        {
            var raw = user?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : null;
        }

        public static Guid? GetPerfilId(ClaimsPrincipal user)
        {
            var raw = user?.FindFirstValue("idPerfil");
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }
}

