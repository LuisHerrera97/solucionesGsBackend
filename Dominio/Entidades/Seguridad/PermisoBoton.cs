using System;

namespace FinancieraSoluciones.Domain.Entidades.Seguridad
{
    public class PermisoBoton
    {
        public Guid Id { get; set; }
        public Guid IdPerfil { get; set; }
        public Guid IdBoton { get; set; }
        public bool TienePermiso { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public Perfil Perfil { get; set; }
        public Boton Boton { get; set; }
    }
}
