using System;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.Finanzas;
using FinancieraSoluciones.Domain.Interfaces.Finanzas;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public class ObtenerResumenDashboardCasoUso
    {
        private readonly ICreditoRepositorio _creditoRepositorio;
        private readonly IClienteRepositorio _clienteRepositorio;

        public ObtenerResumenDashboardCasoUso(ICreditoRepositorio creditoRepositorio, IClienteRepositorio clienteRepositorio)
        {
            _creditoRepositorio = creditoRepositorio;
            _clienteRepositorio = clienteRepositorio;
        }

        public async Task<DashboardResumenDto> Ejecutar(Guid? zonaId = null, bool aplicarFiltroZona = false)
        {
            var hoy = DateTime.Today;

            // 1. Total Vencido (Fichas vencidas)
            // Para ser eficiente, podríamos pedir solo la suma a la base de datos, 
            // pero el repo actual ya filtra por vencidas.
            var fichasVencidas = await _creditoRepositorio.GetFichasVencidasAsync(hoy);
            
            // Si hay filtro de zona, debemos filtrar las fichas por el crédito asociado
            // Pero GetFichasVencidasAsync no soporta zona actualmente.
            // Por simplicidad para este fix, calcularemos el total global o filtrado en memoria si es necesario.
            // Sin embargo, el requerimiento del usuario es "eficiente".
            
            decimal totalVencido = 0;
            if (aplicarFiltroZona)
            {
                totalVencido = fichasVencidas
                    .Where(f => f.Credito?.Cliente?.IdZona == zonaId)
                    .Sum(f => f.Total - f.AbonoAcumulado);
            }
            else
            {
                totalVencido = fichasVencidas.Sum(f => f.Total - f.AbonoAcumulado);
            }

            // 2. Créditos Activos
            // Podemos usar GetAllAsync con filtros si es necesario, pero queremos los totales reales.
            var todosCreditos = await _creditoRepositorio.GetAllAsync(null, null, null, zonaId, aplicarFiltroZona);
            var creditosActivos = todosCreditos.Count(c => c.Estatus == "Activo");

            // 3. Clientes Totales
            var clientes = await _clienteRepositorio.GetAllAsync(1, 1, null, zonaId, aplicarFiltroZona);
            var totalClientes = clientes.TotalCount;

            return new DashboardResumenDto
            {
                TotalClientes = totalClientes,
                CreditosActivos = creditosActivos,
                TotalVencido = totalVencido
            };
        }
    }
}
