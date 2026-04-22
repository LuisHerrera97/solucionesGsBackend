using System;
using System.Threading.Tasks;
using FinancieraSoluciones.Domain.Entidades.Finanzas;

namespace FinancieraSoluciones.Application.Servicios.Finanzas
{
    public interface ICreditoZonaAutorizacionService
    {
        Task AsegurarPuedeOperarAsync(Credito credito, Guid? usuarioId);
    }
}
