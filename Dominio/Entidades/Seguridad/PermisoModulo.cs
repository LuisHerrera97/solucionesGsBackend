using System;

namespace FinancieraSoluciones.Domain.Entidades.Seguridad
{
    public class PermisoModulo
    {
        public Guid Id { get; set; }
        public Guid IdPerfil { get; set; }
        public Guid IdModulo { get; set; }
        public bool TienePermiso { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public Perfil Perfil { get; set; }
        public Modulo Modulo { get; set; }
    }
}
