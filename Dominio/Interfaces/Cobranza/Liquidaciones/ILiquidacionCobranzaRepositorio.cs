using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Cobranza.Liquidaciones;

namespace FinancieraSoluciones.Domain.Interfaces.Cobranza.Liquidaciones
{
    public interface ILiquidacionCobranzaRepositorio
    {
        Task<LiquidacionCobranza> AddAsync(LiquidacionCobranza liquidacion);
        Task<LiquidacionCobranza> GetByIdAsync(Guid id);
        Task<IEnumerable<LiquidacionCobranza>> GetByCobradorAsync(Guid cobradorId, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<LiquidacionCobranza>> GetTodasAsync(DateTime fechaInicio, DateTime fechaFin, Guid? zonaId = null);
        Task UpdateAsync(LiquidacionCobranza liquidacion);
        Task<Dictionary<Guid, string>> GetEstatusPorIdsAsync(IEnumerable<Guid> ids);
    }
}

