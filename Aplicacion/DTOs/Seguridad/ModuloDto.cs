using System;
using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class ModuloDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public string Icono { get; set; }
        public bool Activo { get; set; }
        public bool TienePermiso { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Orden { get; set; }
        public List<PaginaDto> Paginas { get; set; } = new List<PaginaDto>();
    }
}
