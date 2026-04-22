using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas;

namespace FinancieraSoluciones.Domain.Interfaces.Finanzas
{
    public interface ICreditoRepositorio
    {
        Task<IEnumerable<Credito>> GetAllAsync(string? searchTerm = null, int? page = null, int? pageSize = null, Guid? zonaId = null, bool aplicarFiltroZona = false);
        Task<Credito> GetByIdAsync(Guid id);
        Task<IEnumerable<Credito>> GetByClienteIdAsync(Guid clienteId);
        Task<int> ContarPorClienteIdAsync(Guid clienteId);
        Task<string> ObtenerSiguienteFolioAsync(DateTime fecha);
        Task<IEnumerable<Ficha>> GetFichasVencidasAsync(DateTime hoy);
        Task<IEnumerable<Ficha>> GetFichasPendientesParaMoraAsync(DateTime hoy);
        Task<Credito> AddAsync(Credito credito);
        Task UpdateAsync(Credito credito);
    }
}
