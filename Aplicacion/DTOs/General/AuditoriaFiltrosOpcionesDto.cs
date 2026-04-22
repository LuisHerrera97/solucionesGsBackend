using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.General
{
    public class OpcionFiltroAuditoriaDto
    {
        public string Valor { get; set; }
        public string Etiqueta { get; set; }
    }

    public class AuditoriaFiltrosOpcionesDto
    {
        public IReadOnlyList<OpcionFiltroAuditoriaDto> Acciones { get; set; }
        public IReadOnlyList<OpcionFiltroAuditoriaDto> EntidadesTipo { get; set; }
    }
}
