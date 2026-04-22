using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Seguridad;

namespace FinancieraSoluciones.Domain.Interfaces.Seguridad
{
    public interface IPasswordHistoryRepositorio
    {
        Task<PasswordHistory> AddAsync(PasswordHistory history);
        Task<IEnumerable<PasswordHistory>> GetRecentAsync(Guid usuarioId, int take);
    }
}

