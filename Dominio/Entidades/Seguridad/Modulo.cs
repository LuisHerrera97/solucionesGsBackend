using System;

namespace FinancieraSoluciones.Domain.Entidades.Seguridad
{
    public class Modulo
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Clave { get; set; }
        public string Icono { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Orden { get; set; }
    }
}