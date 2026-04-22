using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.General;

namespace FinancieraSoluciones.Domain.Interfaces.General
{
    public interface IConfiguracionSistemaRepositorio
    {
        Task<ConfiguracionSistema> GetAsync();
        Task<ConfiguracionSistema> AddAsync(ConfiguracionSistema configuracion);
        Task UpdateAsync(ConfiguracionSistema configuracion);
    }
}

