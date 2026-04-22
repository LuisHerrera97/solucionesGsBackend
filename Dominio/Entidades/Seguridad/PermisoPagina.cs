using System;

namespace FinancieraSoluciones.Domain.Entidades.Seguridad
{
    public class PermisoPagina
    {
        public Guid Id { get; set; }
        public Guid IdPerfil { get; set; }
        public Guid IdPagina { get; set; }
        public bool TienePermiso { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public Perfil Perfil { get; set; }
        public Pagina Pagina { get; set; }
    }
}
