using System;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class PermisoBotonDto
    {
        public Guid Id { get; set; }
        public Guid IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public Guid IdBoton { get; set; }
        public string NombreBoton { get; set; }
        public string ClaveBoton { get; set; }
        public Guid IdPagina { get; set; }
        public string NombrePagina { get; set; }
        public bool TienePermiso { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public int OrdenBoton { get; set; }
    }
}