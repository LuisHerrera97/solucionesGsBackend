using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using FinancieraSoluciones.Domain.Entidades.General;

namespace FinancieraSoluciones.Domain.Interfaces.General
{
    public interface IAuditoriaEventoRepositorio
    {
        Task<AuditoriaEvento> AddAsync(AuditoriaEvento evento);
        Task<IEnumerable<AuditoriaEvento>> GetAsync(DateTime desdeUtc, DateTime hastaUtc, Guid? usuarioId, string accion, string entidadTipo, Guid? entidadId, int? page, int? pageSize);
        Task<IReadOnlyList<string>> GetDistinctAccionesAsync(DateTime desdeUtc, DateTime hastaUtc);
        Task<IReadOnlyList<string>> GetDistinctEntidadTiposAsync(DateTime desdeUtc, DateTime hastaUtc);
    }
}
