using System;

namespace FinancieraSoluciones.Application.DTOs.Seguridad
{
    public class PermisoModuloDto
    {
        public Guid Id { get; set; }
        public Guid IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public Guid IdModulo { get; set; }
        public string NombreModulo { get; set; }
        public string ClaveModulo { get; set; }
        public string IconoModulo { get; set; }
        public bool TienePermiso { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public int OrdenModulo { get; set; }
    }
}