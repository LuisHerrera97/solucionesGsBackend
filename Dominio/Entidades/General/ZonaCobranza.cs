using System;

namespace FinancieraSoluciones.Domain.Entidades.General
{
    public class ZonaCobranza
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public int Orden { get; set; }
    }
}

