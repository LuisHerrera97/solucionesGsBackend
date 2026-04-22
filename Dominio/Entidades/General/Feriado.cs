using System;

namespace FinancieraSoluciones.Domain.Entidades.General
{
    public class Feriado
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}

