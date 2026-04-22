using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.General;
using FinancieraSoluciones.Domain.Interfaces.General;
using FinancieraSoluciones.Infraestructura.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancieraSoluciones.Infraestructura.Repositorios.General
{
    public class ConfiguracionSistemaRepositorio : IConfiguracionSistemaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public ConfiguracionSistemaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracionSistema> GetAsync()
        {
            return await _context.ConfiguracionSistema.FirstOrDefaultAsync();
        }

        public async Task<ConfiguracionSistema> AddAsync(ConfiguracionSistema configuracion)
        {
            await _context.ConfiguracionSistema.AddAsync(configuracion);
            return configuracion;
        }

        public async Task UpdateAsync(ConfiguracionSistema configuracion)
        {
            _context.ConfiguracionSistema.Update(configuracion);
            await Task.CompletedTask;
        }
    }
}
