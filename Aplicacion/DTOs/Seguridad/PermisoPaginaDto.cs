using System;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class PermisoPaginaDto
    {
        public Guid Id { get; set; }
        public Guid IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public Guid IdPagina { get; set; }
        public string NombrePagina { get; set; }
        public string ClavePagina { get; set; }
        public string RutaPagina { get; set; }
        public Guid IdModulo { get; set; }
        public string NombreModulo { get; set; }
        public bool TienePermiso { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public int OrdenPagina { get; set; }
    }
}