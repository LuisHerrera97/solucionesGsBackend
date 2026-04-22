using System;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class BotonDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public Guid IdPagina { get; set; }
        public string NombrePagina { get; set; }
        public bool Activo { get; set; }
        public bool TienePermiso { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Orden { get; set; }
    }
}
