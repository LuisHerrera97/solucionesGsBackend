using System;
using System.Collections.Generic;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class PaginaDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public string Ruta { get; set; }
        public Guid IdModulo { get; set; }
        public string NombreModulo { get; set; }
        public bool Activo { get; set; }
        public bool TienePermiso { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Orden { get; set; }
        public List<BotonDto> Botones { get; set; } = new List<BotonDto>();
    }
}
