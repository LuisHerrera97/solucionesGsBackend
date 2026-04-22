using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface ITokenService
    {
        string GenerateAccessToken(Usuario usuario);
        string GenerateRefreshToken();
    }
}
