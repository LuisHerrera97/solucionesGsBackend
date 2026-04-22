using System;

namespace FinancieraSoluciones.Domain.Entidades.General
{
    public class AuditoriaEvento
    {
        public Guid Id { get; set; }
        public Guid? UsuarioId { get; set; }
        public string Accion { get; set; }
        public string EntidadTipo { get; set; }
        public Guid? EntidadId { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; }
    }
}

