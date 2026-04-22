using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.Finanzas
{
    public class ClientesListadoDto
    {
        public List<ClienteDto> Items { get; set; } = new List<ClienteDto>();
        public int TotalCount { get; set; }
    }
}
