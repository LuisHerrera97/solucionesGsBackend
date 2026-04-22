namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}
