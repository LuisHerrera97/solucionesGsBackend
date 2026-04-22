using System;
using System.Linq;
using System.Threading.Tasks;
using FinancieraSoluciones.Application.DTOs.General;
using FinancieraSoluciones.Application.General;
using FinancieraSoluciones.Domain.Interfaces.General;

namespace FinancieraSoluciones.Application.CasosUso.General
{
    public class ObtenerAuditoriaFiltrosOpcionesCasoUso
    {
        private readonly IAuditoriaEventoRepositorio _auditoriaRepositorio;

        public ObtenerAuditoriaFiltrosOpcionesCasoUso(IAuditoriaEventoRepositorio auditoriaRepositorio)
        {
            _auditoriaRepositorio = auditoriaRepositorio;
        }

        public async Task<AuditoriaFiltrosOpcionesDto> Ejecutar(DateTime desdeUtc, DateTime hastaUtc)
        {
            var acciones = await _auditoriaRepositorio.GetDistinctAccionesAsync(desdeUtc, hastaUtc);
            var tipos = await _auditoriaRepositorio.GetDistinctEntidadTiposAsync(desdeUtc, hastaUtc);

            return new AuditoriaFiltrosOpcionesDto
            {
                Acciones = acciones
                    .Select(v => new OpcionFiltroAuditoriaDto
                    {
                        Valor = v,
                        Etiqueta = AuditoriaFiltrosEtiquetas.EtiquetaAccion(v),
                    })
                    .OrderBy(o => o.Etiqueta, StringComparer.OrdinalIgnoreCase)
                    .ToList(),
                EntidadesTipo = tipos
                    .Select(v => new OpcionFiltroAuditoriaDto
                    {
                        Valor = v,
                        Etiqueta = AuditoriaFiltrosEtiquetas.EtiquetaEntidadTipo(v),
                    })
                    .OrderBy(o => o.Etiqueta, StringComparer.OrdinalIgnoreCase)
                    .ToList(),
            };
        }
    }
}
