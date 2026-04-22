using System;

namespace FinancieraSoluciones.Application.DTOs.General
{
    public class AuditoriaEventoDto
    {
        public Guid Id { get; set; }
        public Guid? UsuarioId { get; set; }
        /// <summary>Nombre legible del usuario que realizó la acción (si aplica).</summary>
        public string UsuarioNombre { get; set; }
        public string Accion { get; set; }
        public string EntidadTipo { get; set; }
        public Guid? EntidadId { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; }
    }
}

